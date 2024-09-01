using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("��������")]
    public float maxHealth;
    public float currentHealth;

    [Header("�����޵�")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    [Header("�¼�")]
    public UnityEvent<Transform> OnTakeDamage;//���˵��¼�
    public UnityEvent OnDie;//�����¼�

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        //�޵�ʱ
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }

    /// <summary>
    /// �˺�����
    /// </summary>
    /// <param name="attacker"></param>
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) return;

        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            //���˺��޵�
            TriggerInvulnerable();

            //��������
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //��������
            OnDie?.Invoke();
        }
    }

    /// <summary>
    /// �����޵�
    /// </summary>
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}
