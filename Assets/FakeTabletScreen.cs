﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CRI.HelloHouston.Experience
{
    public class FakeTabletScreen : XPElement
    {
        [SerializeField]
        private FakeSynchronizer _synchronizer;
        [SerializeField]
        private GameObject _panel, _b1C2, _b1C4, _b1C4Left, _b1C5Left, _b1C6Left;
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private float _speed = 0.2f;
        [SerializeField]
        private string _realPassword;
        public string enteredPassword;
        public string realParticles;
        public string[] _enteredParticles;
        [SerializeField]
        private Button[] _digitButtons, _particleButtons;
        public string[] result;
        
        

        IEnumerator FakeLoading()
        {
            float i = 0f;
            while (_slider.value <= 0.75f)
            {
                i += Time.deltaTime * _speed;
                _slider.value = Mathf.Sqrt(i);
                yield return null;
            }
        }

        public void ClearParticles()
        {
            for(int i = 0; i < _enteredParticles.Length; i++)
            {
                _enteredParticles[i] = "";
            }
            _synchronizer.SynchronizeScreens("EnteringParticle");
        }

        public void SubmitParticles()
        {
            string particles = "";

            for (int i = 0; i < _enteredParticles.Length; i++) 
            {
                particles+=_enteredParticles[i];
            }

            if (particles == realParticles)
            {
                _synchronizer.SynchronizeScreens("ParticleCorrect");
            }
            else if (particles != realParticles)
            {
                if (particles.Length != realParticles.Length)
                {

                }
                else
                {
                    string[] particleBreak = new string[realParticles.Length];
                    particleBreak = realParticles.Split(" "[0]);

                    for(int i = 0; i < particleBreak.Length; i++)
                    {
                        //if(particleBreak)
                    }
                }
                  
                _synchronizer.SynchronizeScreens("ParticleInCorrect");
                enteredPassword = "";
            }
        }

        public void EnteringParticle(string particle)
        {
            Debug.Log("touched");
            for(int i = 0; i < _enteredParticles.Length; i++)
            {
                if(_enteredParticles[i] == "")
                {
                    _enteredParticles[i] = particle;
                    _synchronizer.SynchronizeScreens("EnteringParticle");
                    Debug.Log(particle);
                    break;
                }
            }
        }

        public void EnteringDigit(int number)
        {
            if(enteredPassword.Length < _realPassword.Length)
            {
                enteredPassword += number.ToString();
                _synchronizer.SynchronizeScreens("EnteringDigit");

                if (enteredPassword.Length == _realPassword.Length && enteredPassword == _realPassword)
                {
                    _synchronizer.SynchronizeScreens("PasswordCorrect");
                }
                else if(enteredPassword.Length == _realPassword.Length && enteredPassword != _realPassword)
                {
                    _synchronizer.SynchronizeScreens("PasswordInCorrect");
                    enteredPassword = "";
                }
            }
        }

        public void AccessGranted()
        {
            _b1C5Left.SetActive(false);
            _b1C6Left.SetActive(true);
        }

        public void WaitingConfirmation()
        {
            _b1C2.SetActive(true);
            _panel.SetActive(false);
        }

        public void OverrideButtonClicked()
        {
            _synchronizer.SynchronizeScreens("OverrideButtonClicked");
            _b1C5Left.SetActive(true);
            _b1C4Left.SetActive(false);
        }

        public void StartButtonClicked()
        {
            _synchronizer.SynchronizeScreens("StartButtonClicked");
            _b1C4.SetActive(true);
            _b1C2.SetActive(false);
            StartCoroutine("FakeLoading");
        }

        public override void OnResolved()
        {
            Debug.Log(name + "Resolved");
        }

        public override void OnFailed()
        {
            Debug.Log(name + "Failed");
        }

        public override void OnActivated()
        {
            Debug.Log(name + "Activated");
        }

        public override void OnPause()
        {
            Debug.Log(name + "Paused");
        }

        public override void OnUnpause()
        {
            Debug.Log(name + "Unpaused");
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
