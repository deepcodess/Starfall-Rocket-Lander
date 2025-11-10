using Unity.Cinemachine;
using UnityEngine;

public class GameManagerVisuals : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource crashCinemachineImpulseSource;


    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.TooSteepAngle:
            case Lander.LandingType.TooFastLanding:
            case Lander.LandingType.WrongLandingArea:
                //CRASH
                crashCinemachineImpulseSource.GenerateImpulse(45f);
                break;
            case Lander.LandingType.Success:
                break;
        }
    }
}
