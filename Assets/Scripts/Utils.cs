using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Utils
    {
        public static void SetLayerRecursive(GameObject gameObject, int newLayer)
        {
            if (gameObject == null)
            {
                return;
            }

            gameObject.layer = newLayer;

            foreach (Transform transform in gameObject.transform)
            {
                if (transform == null)
                {
                    continue;
                }

                SetLayerRecursive(transform.gameObject, newLayer);
            }
        }
    }
}