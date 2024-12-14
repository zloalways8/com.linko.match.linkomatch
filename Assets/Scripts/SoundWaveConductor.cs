using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundWaveConductor : MonoBehaviour
{
    [SerializeField] private AudioMixer _resonanceCore;
    [SerializeField] private Image waveToggleGlyph;
    [SerializeField] private Image melodyToggleGlyph;
    [SerializeField] private Sprite pulseOnGlyph;
    [SerializeField] private Sprite pulseOffGlyph;
    [SerializeField] private Sprite pulseOnSigil;
    [SerializeField] private Sprite pulseOffSigil;

    private bool _echoPulseActive;
    private bool _harmonyThread;

    void Start()
    {
        _echoPulseActive = PlayerPrefs.GetInt("sonicEchoToggled", 1) == 1;
        _harmonyThread = PlayerPrefs.GetInt("_melodicFlowActive", 1) == 1;
        
        SoundWarpGrid();
        TuneShifter();
        PauseMark();
    }

    public void SonicFluxToggle()
    {
        _echoPulseActive = !_echoPulseActive;
        SoundWarpGrid();
        PauseMark();
    }

    public void ResonanceFlux()
    {
        _harmonyThread = !_harmonyThread;
        TuneShifter();
        PauseMark();
    }

    private void SoundWarpGrid()
    {
        _resonanceCore.SetFloat(AvatarPhaseSetup.REVERB_TRACE, _echoPulseActive ? 0f : -80f);
        waveToggleGlyph.sprite = _echoPulseActive ? pulseOnGlyph : pulseOffGlyph;
    }

    private void TuneShifter()
    {
        _resonanceCore.SetFloat(AvatarPhaseSetup.AUDIO_SIGNAL_POINT, _harmonyThread ? 0f : -80f);
        melodyToggleGlyph.sprite = _harmonyThread ? pulseOnSigil : pulseOffSigil;
    }
    
    public void PauseMark()
    {
        PlayerPrefs.SetInt("sonicEchoToggled", _echoPulseActive ? 1 : 0);
        PlayerPrefs.SetInt("_melodicFlowActive", _harmonyThread ? 1 : 0);
        PlayerPrefs.Save();
    }
}