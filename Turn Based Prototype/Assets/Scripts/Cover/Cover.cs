using System;
using System.Collections.Generic;
using Cover;
using UnityEngine;

namespace CoverSystem
{
    public class Cover : MonoBehaviour
    {
        
        [SerializeField] private CoverType coverType;
        [SerializeField] private CoverDurability coverDurability;
        
        private void OnTriggerStay(Collider other)
        {
            if(other.TryGetComponent<Unit>(out var unit))
            {
                if(unit.TryGetComponent<CoverAbility>(out var coverAbility))
                {
                    coverAbility.SetCover(this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.TryGetComponent<Unit>(out var unit))
            {
                if(unit.TryGetComponent<CoverAbility>(out var coverAbility))
                {
                    coverAbility.SetOutOfCover();
                }
            }
        }
        
        public CoverType GetCoverType()
        {
            return coverType;
        }

        public CoverDurability GetCoverDurability()
        {
            return coverDurability;
        }
        
    }
}