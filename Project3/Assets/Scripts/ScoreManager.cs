using System;
using UnityEngine;

/// <summary>
/// Lightweight score manager singleton. Subscribe to OnScoreChanged to update UI.
/// </summary>
public class ScoreManager : MonoBehaviour
{
	public static ScoreManager Instance { get; private set; }

	public event Action<int> OnScoreChanged;

	public int Score { get; private set; }

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void AddScore(int amount)
	{
		if (amount == 0) return;
		Score += amount;
		OnScoreChanged?.Invoke(Score);
		Debug.Log($"Score += {amount} -> {Score}");
	}

	public void ResetScore()
	{
		Score = 0;
		OnScoreChanged?.Invoke(Score);
	}
}