using UnityEngine;
using System;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	private const float OneMinuteInSeconds = 3600f; // 24 Horas em segundos
	private const float OneDayInSeconds = 86400;
	private const float DayStartHour = 6f;
	private const float NightStartHour = 18f;

	public ReflectionProbe TerrainReflectionProbe;
	public ParticleSystem StarField;
	public Material SceneSkyBoxMat;
	private Material _clonedSceneSkyBoxMat;
	public Light SunLight;
	public Light MoonLight;
	public Gradient DayNightCycleColor;
	public AnimationCurve Thickness;
	public AnimationCurve Exposure;
	public UnityEngine.UI.Text HourDisplayText;
	public float OneDayInMinutes;
	public float SecondScale;
	public float CurrentTimeInSeconds;
	public int Hour;
	public int Minute;
	public int Second;
	public float Cross;
	public float StartAtHour;

	private float _lastTime;
	private float _defaultSunSize;
	private bool _sunLigthRotationInitialized;

	void Awake()
	{
		SceneSkyBoxMat = RenderSettings.skybox;

		if (SceneSkyBoxMat != null)
		{
			_defaultSunSize = SceneSkyBoxMat.GetFloat("_SunSize");
		}

		StartAtHour *= 60 * 60; // Hora * Minuto * Segundo
		CurrentTimeInSeconds = StartAtHour;

		// Clone de Skybox
		_clonedSceneSkyBoxMat = new Material(SceneSkyBoxMat);
	}		

	// Update is called once per frame
	void Update () {		
		SetSecondScale();
		CalculateWorldCurrentTime();
		SetLightColor();
		SetSkyBox();
		SetStars();
		SetDisplayHour();

		if (TerrainReflectionProbe != null)
		{
			//if (Hour%2 == 0)
			//	TerrainReflectionProbe.RenderProbe();
		}
	}

	void SetStars()
	{
		if (StarField != null)
		{
			if (Hour > 16) StarField.Play(true);
			else if (Hour > 8) {
				StarField.Stop(true);
				StarField.Clear(true);
			}			
		}
	}

	/// <summary>
	/// Define a escala de segundos a ser utilizada
	/// </summary>
	void SetSecondScale()
	{
		SecondScale = 24 * OneMinuteInSeconds  / (OneDayInMinutes * 60);
		if (float.IsInfinity(SecondScale) || SecondScale < 0)
			SecondScale = 0;


	}

	/// <summary>
	/// Calcula o tempo atual de acordo com a escala de segundo
	/// </summary>
	void CalculateWorldCurrentTime()
	{

		CurrentTimeInSeconds += (Time.time - _lastTime) * SecondScale;

		if (CurrentTimeInSeconds > OneDayInSeconds)
			CurrentTimeInSeconds = 0;

		TimeSpan _currentTime = TimeSpan.FromSeconds(CurrentTimeInSeconds);

		Hour = _currentTime.Hours;
		Minute = _currentTime.Minutes;
		Second = _currentTime.Seconds;

		_lastTime = Time.time;		

		Cross = CurrentTimeInSeconds / OneDayInSeconds;
	}

	/// <summary>
	/// Metodo responsavel por definir a cor do ponto de luz fornecido
	/// </summary>
	void SetLightColor()
	{
		float rotateTo = 360f * (Cross - 0.25f);

		if (SunLight != null)
		{
			if (Hour >= 6 && !SunLight.enabled) SunLight.enabled = true;
			if (Hour >= 18 && SunLight.enabled) SunLight.enabled = false;

			SunLight.color = DayNightCycleColor.Evaluate(Cross);

			SunLight.transform.rotation = Quaternion.Euler(rotateTo, 270, 0);

			RenderSettings.ambientSkyColor = SunLight.color;

		}

		if (MoonLight != null)
		{
			if (Hour >= 18 && !MoonLight.enabled) MoonLight.enabled = true;
			if (Hour >= 6 && MoonLight.enabled) MoonLight.enabled = false;

			MoonLight.color = DayNightCycleColor.Evaluate(Cross);

			MoonLight.transform.rotation = Quaternion.Euler(rotateTo * -1, 270 * -1, 0);

			RenderSettings.ambientSkyColor = SunLight.color;
		}
	}

	/// <summary>
	/// Metodo responsavel por definir os dados do SkyBox
	/// </summary>
	void SetSkyBox()
	{
		if (_clonedSceneSkyBoxMat != null)
		{
			_clonedSceneSkyBoxMat.SetFloat("_AtmosphereThickness", Thickness.Evaluate(Cross));
			_clonedSceneSkyBoxMat.SetFloat("_Exposure", Exposure.Evaluate(Cross));
		}
	}

	/// <summary>
	/// Metodo para apresentar o Horario Atual do Jogo
	/// </summary>
	void SetDisplayHour()
	{
		if (HourDisplayText != null)
		{
			HourDisplayText.text = string.Format("{0}:{1}:{2}", Hour.ToString("00"), Minute.ToString("00"), Second.ToString("00"));
		}
	}
}
