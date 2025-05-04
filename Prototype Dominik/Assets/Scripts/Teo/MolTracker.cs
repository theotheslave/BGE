using System.Collections.Generic;
using UnityEngine;

public class MolTracker : MonoBehaviour
{
    private HashSet<MoleculeParticle> inside = new HashSet<MoleculeParticle>();

    public int GetMoleculeCount()
    {
        return inside.Count;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var mol = other.GetComponent<MoleculeParticle>();
        if (mol != null)
        {
            inside.Add(mol);
            Debug.Log($"✅ Molecule entered chamber: {mol.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var mol = other.GetComponent<MoleculeParticle>();
        if (mol != null && inside.Contains(mol))
        {
            inside.Remove(mol);
           // Debug.Log($"❌ Molecule EXITED: {mol.name}");
        }
    }
}
