﻿using UnityEngine;

public class Bonus : MonoBehaviour {

    [SerializeField] GameManager.e_bonusType m_bonusType;
    [SerializeField] float m_bonusTime;

    // PUBLIC METHODES

    public float BonusTime
    {
        get
        {
            return m_bonusTime;
        }
    }

    public GameManager.e_bonusType BonusType
    {
        get
        {
            return m_bonusType;
        }
    }
}
