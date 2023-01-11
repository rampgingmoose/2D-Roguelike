using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader materilizeShader, Color materializeColor, float materializeTime, 
        SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Material materilizeMaterial = new Material(materilizeShader);

        materilizeMaterial.SetColor("_EmissionColor", materializeColor);

        foreach(SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materilizeMaterial;
        }

        float dissolveAmount = 0f;

        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;

            materilizeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

            yield return null;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
        }
    }
}
