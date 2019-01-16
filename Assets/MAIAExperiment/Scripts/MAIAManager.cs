﻿using CRI.HelloHouston.Calibration;
using System.Collections;
using UnityEngine;

/// <summary>
/// The synchronizer of the particle physics experiment.
/// </summary>
namespace CRI.HelloHouston.Experience.MAIA
{
    public class MAIAManager : XPManager
    {
        /// <summary>
        /// The top left script of the experiment block.
        /// </summary>
        private MAIATopScreen _topScreen;
        /// <summary>
        /// The top right script of the experiment block.
        /// </summary>
        private MAIATubeScreen _tubeScreen;
        /// <summary>
        /// The tablet script of the experiment block.
        /// </summary>
        private MAIATabletScreen _tabletScreen;
        /// <summary>
        /// The hologram scripts of the table block.
        /// </summary>
        private MAIAHologram[] _holograms;

        /// <summary>
        /// Activates the manual override panel of the tablet.
        /// </summary>
        public void ManualOverrideActive()
        {
            _tabletScreen.ManualOverride();
        }

        /// <summary>
        /// Directly skips to the Feynman diagrams step.
        /// </summary>
        public void SkipStepOne()
        {
            _holograms[0].ActivateHologram(true);
            _tabletScreen.SkipStepOne();
            _tubeScreen.SkipStepOne();
            _topScreen.SkipStepOne();
        }

        /// <summary>
        /// Tells the tablet that the experiment has finished loading.
        /// </summary>
        public void LoadingBarFinished()
        {
            _tabletScreen.WaitingConfirmation();
        }

        /// <summary>
        /// Tells the main screen that the start button has been pressed.
        /// </summary>
        public void StartButtonClicked()
        {
            _topScreen.ManualOverride();
        }

        /// <summary>
        /// Tells the main screen that the override button has been pressed.
        /// </summary>
        public void OverrideButtonClicked()
        {
            _topScreen.AccessCode();
        }

        /// <summary>
        /// Tells the main screen that the right password has been entered.
        /// </summary>
        public void CorrectPassword()
        {
            _topScreen.Access(true);
        }

        /// <summary>
        /// Tells the main screen that an incorrect password has been entered.
        /// </summary>
        public void IncorrectPassword()
        {
            _topScreen.Access(false);
        }

        /// <summary>
        /// Tells the main screen that a password digit has been entered.
        /// </summary>
        public void EnteringDigit()
        {
            _topScreen.DisplayPassword(_tabletScreen.enteredPassword);
        }

        /// <summary>
        /// Tells the tablet that access to the experiment has been granted.
        /// </summary>
        public void AccessGranted()
        {
            _holograms[0].ActivateHologram(true);
            _tabletScreen.AccessGranted();
            _tabletScreen.reactionExits = _tabletScreen.ParticlesCombination();
            _topScreen.FillNbParticlesDetected(_tabletScreen.reactionExits);
        }

        /// <summary>
        /// Tells the main screen that a particle has been entered.
        /// </summary>
        public void EnteringParticles()
        {
            _topScreen.DisplayParticles(_tabletScreen._enteredParticles);
        }

        /// <summary>
        /// Tells the main screen that the correct combination of particles has been entered.
        /// </summary>
        public void CorrectParticle()
        {
            _holograms[0].AnimHologram(_tabletScreen.reactionExits);
            _holograms[0].DisplaySplines();
            _topScreen.ParticleGrid(_tabletScreen.reactionExits);
            _topScreen.FillParticlesTable(_tabletScreen.nbAntielectron, _topScreen._textAntielectron);
            _topScreen.FillParticlesTable(_tabletScreen.nbAntimuon, _topScreen._textAntimuon);
            _topScreen.FillParticlesTable(_tabletScreen.nbAntiquark, _topScreen._textAntiquark);
            _topScreen.FillParticlesTable(_tabletScreen.nbElectron, _topScreen._textElectron);
            _topScreen.FillParticlesTable(_tabletScreen.nbMuon, _topScreen._textMuon);
            _topScreen.FillParticlesTable(_tabletScreen.nbNeutrino, _topScreen._textNeutrino);
            _topScreen.FillParticlesTable(_tabletScreen.nbPhoton, _topScreen._textPhoton);
            _topScreen.FillParticlesTable(_tabletScreen.nbQuark, _topScreen._textQuark);
            _topScreen.FillChosenDiagrams(_tabletScreen._chosenReactions, _tabletScreen._realReaction);
            _topScreen.FillInteractionType(_tabletScreen._realReaction);
        }

        /// <summary>
        /// Tells the main screen to clear all the entered particles.
        /// </summary>
        public void ClearParticles()
        {
            _topScreen.ClearParticles();
        }

        /// <summary>
        /// Tells the main screen to clear the last entered particles.
        /// </summary>
        public void DeleteParticle()
        {
            _topScreen.DeleteParticle(_tabletScreen._enteredParticles.Count);
        }

        /// <summary>
        /// Tells the tube screen to display another Feynman diagram.
        /// </summary>
        public void OtherDiagram()
        {
            _tubeScreen.OtherDiagram(_tabletScreen.displayedDiagram, _tabletScreen._allReactions);
        }

        /// <summary>
        /// Tells the tube screen to mark a Feynman diagram for its exits.
        /// </summary>
        public void SelectExit()
        {
            _tubeScreen.SelectExit(_tabletScreen.displayedDiagram);
        }

        /// <summary>
        /// Tells the tube screen to mark a Feynman diagram for its interactions.
        /// </summary>
        public void SelectInteraction()
        {
            _tubeScreen.SelectInteraction(_tabletScreen.displayedDiagram);
        }

        /// <summary>
        /// Tells the top screen that a combination of particles with a wrong length has been entered.
        /// </summary>
        public void ParticleWrongLength()
        {
            _topScreen.ErrorParticles(_tabletScreen.particleErrorString);
        }

        /// <summary>
        /// Tells the top screen that a combination of particles with the wrong symbols has been entered.
        /// </summary>
        public void ParticleWrongSymbol()
        {
            _topScreen.ErrorParticles(_tabletScreen.particleErrorString);
        }

        /// <summary>
        /// Tells the top screen that a combination of particles with the wrong charges has been entered.
        /// </summary>
        public void ParticleWrongCharge()
        {
            _topScreen.ErrorParticles(_tabletScreen.particleErrorString);
        }

        /// <summary>
        /// Tells every screen that the right combination of particles has been entered.
        /// </summary>
        public void ParticleRightCombination()
        {
            _topScreen.OverrideSecond();
            _tabletScreen.OverrideSecond();
           // _tubeScreen.OverrideSecond(_tabletScreen._allReactions);
        }

        /// <summary>
        /// Tells the main screen that a reaction has been selected.
        /// </summary>
        public void ReactionSelected()
        {
            _topScreen.ReactionSelected(_tabletScreen._realReaction, _tubeScreen.diagramSelected);
        }

        protected override void PreShow(VirtualWallTopZone wallTopZone, ElementInfo[] info)
        {
            base.PreShow(wallTopZone, info);
            _tabletScreen = GetElement<MAIATabletScreen>();
            _topScreen = GetElement<MAIATopScreen>();
            _tubeScreen = GetElement<MAIATubeScreen>();
        }

        protected override void PostInit(XPContext xpContext, ElementInfo[] info, LogExperienceController logController, XPState stateOnActivation)
        {
            base.PostInit(xpContext, info, logController, stateOnActivation);
            _holograms = GetElements<MAIAHologram>();
            _tabletScreen = GetElement<MAIATabletScreen>();
            _topScreen = GetElement<MAIATopScreen>();
            _tubeScreen = GetElement<MAIATubeScreen>();
        }
    }
}