using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer[] renderersToOutline;
    [SerializeField] private int outlineSlot = 1;

    private Material[][] _originalMaterials;

    private void Awake()
    {
        // Save original materials
        _originalMaterials = new Material[renderersToOutline.Length][];
        for (int i = 0; i < renderersToOutline.Length; i++)
        {
            _originalMaterials[i] = renderersToOutline[i].materials;
        }
    }

    public void EnableOutline()
    {
        for (int i = 0; i < renderersToOutline.Length; i++)
        {
            Material[] mats = renderersToOutline[i].materials;
            if (mats.Length > outlineSlot)
            {
                mats[outlineSlot] = outlineMaterial;
                renderersToOutline[i].materials = mats;
            }
        }
    }

    public void DisableOutline()
    {
        for (int i = 0; i < renderersToOutline.Length; i++)
        {
            Material[] mats = renderersToOutline[i].materials;
            if (mats.Length > outlineSlot)
            {
                mats[outlineSlot] = _originalMaterials[i][0];
                renderersToOutline[i].materials = mats;
            }
        }
    }
}
