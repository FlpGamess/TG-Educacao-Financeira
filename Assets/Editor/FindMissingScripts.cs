using UnityEngine;
using UnityEditor;

public class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void Find()
    {
        GameObject[] objetos = Object.FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int encontrados = 0;

        foreach (GameObject obj in objetos)
        {
            Component[] componentes = obj.GetComponents<Component>();

            for (int i = 0; i < componentes.Length; i++)
            {
                if (componentes[i] == null)
                {
                    Debug.LogError(
                        $"SCRIPT FALTANDO: {GetCaminho(obj)}",
                        obj
                    );

                    encontrados++;
                }
            }
        }

        Debug.Log($"Busca finalizada. Scripts faltando: {encontrados}");
    }

    static string GetCaminho(GameObject obj)
    {
        string caminho = obj.name;
        Transform pai = obj.transform.parent;

        while (pai != null)
        {
            caminho = pai.name + "/" + caminho;
            pai = pai.parent;
        }

        return caminho;
    }
}