using System;
using UnityEngine;

public static class FunctionPeriodic
{
    public static void Create(Action action, float repeatRate)
    {
        GameObject obj = new GameObject("FunctionPeriodic");
        FunctionPeriodicMono mono = obj.AddComponent<FunctionPeriodicMono>();
        mono.SetAction(action, repeatRate);
    }

    private class FunctionPeriodicMono : MonoBehaviour
    {
        private Action action;
        private float timer;

        public void SetAction(Action action, float repeatRate)
        {
            this.action = action;
            this.timer = repeatRate;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer += timer <= 0 ? timer * -1f : 0;
                action();
            }
        }
    }
}
