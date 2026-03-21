using System.Collections;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    public abstract IEnumerator Show();
    public abstract IEnumerator Hide();
}
