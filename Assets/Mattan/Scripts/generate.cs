 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generate : MonoBehaviour
{
    
    [SerializeField] bool diffHeights;
    [SerializeField] int mapRadius = 3;
    [SerializeField] float pBadTile = 0.2f;

    [SerializeField] MapPalletteScriptableObject [] pallettes;
    MapPalletteScriptableObject chosenPallette;
    [SerializeField] float scaleFactor = 1;
    
    float heightDeltas = 3.5f;
 
    void SpawnSurface(bool diffHeights){
        int i=0, j=0;
        float x, y;
        for (i = -mapRadius + 1; i < mapRadius; i++)
        {
            for (j = -mapRadius + 1; j < mapRadius; j++)
            {
                if (i + j < mapRadius && i + j > -mapRadius)
                {
                    x = 1.75f * i + (1.75f / 2.0f) * j;
                    y = 1.5f * j;

                    float z = !diffHeights ? 0 : Mathf.Sin((i + j) * 2.2f) / heightDeltas;
                    bool spawnGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(i) < 2) && (Mathf.Abs(j) < 2));
                    GameObject goodTile =  chosenPallette.goodTile;
                    GameObject newTile = Instantiate(
                                            spawnGoodTile ? goodTile : (Random.Range(0f,1f) < 0.5f ? chosenPallette.dmgTile : chosenPallette.windingTile), 
                                            new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor), 
                                            Quaternion.Euler(0, 0, 0));
                    newTile.transform.localScale = newTile.transform.localScale * scaleFactor * 1.1f;
                    newTile.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    newTile.transform.parent = this.transform;
                }

            }
        }
    }

    void PickRandomPallette(){chosenPallette = pallettes[Random.Range(0, pallettes.Length)];}
    
    void Start()
    {  
        PickRandomPallette();
        SpawnSurface(diffHeights);
    }


    public float GetScaleFactor()
    {
        return scaleFactor;
    }

    class Polynom
    {
        public float a, b, c, d;
        public Polynom(float a, float b, float c, float d)
        {
            this.a = a; this.b = b; this.c = c; this.d = d;
        }

        public float calculate(float x)
        {
            a = a * x * x * x;
            b = a * x * x;
            c = a * x;
            d = a;
            return (a + b + c + d);
        }
    }
/*
    class tile
    {
        public int x;
        public int y;
        public int z=0;

        public tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public void setX(int x) { this.x = x; }
        public void setY(int y) { this.y = y; }
        public void setZ(int z) { this.z = z; }
        
    }
*/

}
