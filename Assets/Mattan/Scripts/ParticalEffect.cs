using UnityEngine;

public class ParticalEffect : Effect {
    ParticleSystem effect;
    public override void Apply(){
        if (effect == null)
            effect = GetComponent<ParticleSystem>();
        effect.Play();
    }
}