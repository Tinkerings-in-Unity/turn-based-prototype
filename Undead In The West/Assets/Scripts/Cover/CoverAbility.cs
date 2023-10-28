using System;
using System.Collections.Generic;
using Cover;
using UnityEngine;

namespace CoverSystem
{
    public class CoverAbility : MonoBehaviour
    {

        public event EventHandler OnInCover;
        public event EventHandler OnOutOfCover;

        private Cover _cover;
        private bool _isHiding;
        
        public void SetCover(Cover cover)
        {
            _cover = cover;
            _isHiding = cover.GetCoverType() == CoverType.Bush;
            
            OnInCover?.Invoke(this, EventArgs.Empty);
        }
        
        public Cover GetCover()
        {
            return _cover;
        }

        public void SetOutOfCover()
        {
            _cover = null;
            _isHiding = false;
            
            OnOutOfCover?.Invoke(this, EventArgs.Empty);
        }
        
        public bool IsHiding()
        {
            return _isHiding;
        }
    }
}