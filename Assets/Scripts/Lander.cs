using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Lander : MonoBehaviour
{

    private const float GRAVITY_NORMAL = 0.7f;
    
    //For Singleton Pattern- Game Manager
    public static Lander Instance { get; private set; }
    
    
    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;


    [SerializeField] private float maxFuel = 10f;
    [SerializeField] private float rechargeRate = 1f; // units per second
    private bool isOnRechargePad = false;


    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;

    public event EventHandler OnCoinPickup;
    public event EventHandler OnFuelPickup;

    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float time_taken;
    }

    public event EventHandler<OnStageChangedEventArgs> OnStageChanged;
    public class OnStageChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    private State state;
    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }


    private void Awake()
    {
        Instance = this;
        fuelAmount = maxFuel;
        landerRigidbody2D = GetComponent<Rigidbody2D>();

        landerRigidbody2D.gravityScale = 0f;
        state = State.WaitingToStart;
    }


    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.IsRightActionPressed() || GameInput.Instance.IsLeftActionPressed())
                {
                    //Pressing any key
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:


                if (isOnRechargePad && fuelAmount < maxFuel)
                    fuelAmount = Mathf.Min(maxFuel, fuelAmount + rechargeRate * Time.fixedDeltaTime);


                if (fuelAmount <= 0f)
                {
                    return;//No Fuel left.
                }

                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.IsRightActionPressed() || GameInput.Instance.IsLeftActionPressed())
                {
                    //Pressing any key
                    ConsumeFuel();
                }

                if (GameInput.Instance.IsUpActionPressed())
                {
                    float force = 600f;
                    landerRigidbody2D.AddForce(transform.up * force * Time.deltaTime);

                    OnUpForce?.Invoke(this, EventArgs.Empty); //For Thrusters
                }

                if (GameInput.Instance.IsLeftActionPressed())
                {
                    float turnSpeed = +80f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);

                    OnLeftForce?.Invoke(this, EventArgs.Empty); //For Thrusters
                }

                if (GameInput.Instance.IsRightActionPressed())
                {
                    float turnSpeed = -80f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);

                    OnRightForce?.Invoke(this, EventArgs.Empty); //For Thrusters
                }
                break;


            case State.GameOver:
                break;
        }
    }




    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on the Terrain.");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                score = 0,
                time_taken = GameManager.Instance.GetTime(),
            });

            SetState(State.GameOver);


            return;
        }


        isOnRechargePad = true;

        //SPEED LIMIT LANDING
        float softLandingVelocityMagnitude = 4.5f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            //CRASH
            Debug.Log("Landed Too Fast!");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                score = 0,
                time_taken = GameManager.Instance.GetTime(),
            });

            SetState(State.GameOver);

            return;
        }


        //ANGULAR LANDING (if it landed in straight up or bended region)
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float mindotvector = 0.90f;
        if (dotVector < mindotvector)
        {
            //Landed on a steep angle
            Debug.Log("Landed with a Steep Angle!");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                score = 0,
                time_taken = GameManager.Instance.GetTime(),
            });

            SetState(State.GameOver);

            return;
        }




        //SCORE
        //float scoreDotVectorMultiplier = 10f;
        //float landingAngleScore = Mathf.RoundToInt(maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle);
        //
        //float maxScoreAmountLandingAngle = 100;
        //float maxScoreAmountLandingSpeed = 100;
        //float landingSpeedScore = Mathf.RoundToInt(Mathf.Abs(softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed);
        //
        //Debug.Log("Landing Angle Score: " +  landingAngleScore + "\nLanding Speed Score: " + landingSpeedScore);
        //
        //int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());
        //Debug.Log("Landed Score: " +  score);

        // Check if this landing pad is the goal pad or not
        if (!landingPad.IsGoalPad)
        {
            // This pad is only for refueling, not winning
            Debug.Log("Landed on recharge pad only (No success)");
            return; // Do NOT end the game
        }

        // If it IS a goal pad, then success:
        Debug.Log("Successful Landing!");
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            score = GameManager.Instance.GetScore(),
            time_taken = GameManager.Instance.GetTime(),
        });
        SetState(State.GameOver);

    }



    private void OnCollisionExit2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
            isOnRechargePad = false;
    }



    public float fuelAmountNormalized()
    {
        return fuelAmount / maxFuel;
    }


    //CONSUMPTION OF FUEL
    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    public float GetFuel()
    {
        return fuelAmount;
    }


    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;

            if(fuelAmount > maxFuel)
            {
                fuelAmount = maxFuel;
            }
            OnFuelPickup?.Invoke(this, EventArgs.Empty);
            fuelPickup.DestroySelf();
        }


        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }


    private void SetState(State state)
    {
        this.state = state;
        OnStageChanged?.Invoke(this, new OnStageChangedEventArgs
        {
            state = state
        });
    }
}
