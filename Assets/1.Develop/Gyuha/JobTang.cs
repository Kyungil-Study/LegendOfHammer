using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobTang : MonoBehaviour
{
    public List<Hero> heroes = new List<Hero>();

    private void Start()
    {
        var result = heroes.Select(hero =>
        {
            if (hero.baseAttackDamage > 300)
            {
                return hero;
            }

            return null;
        });

        foreach (var hero in result)
        {
            if (hero != null)
            {
                Debug.Log(hero.name);
            }
            else
            {
                Debug.Log("null");
            }
        }
    }
}
