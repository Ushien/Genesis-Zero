using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectModifier
{
        private Upgrade origin;

        public void SetOrigin(Upgrade newOrigin)
        {
                origin = newOrigin;
        }

        public Upgrade GetOrigin()
        {
                return origin;
        }
}
