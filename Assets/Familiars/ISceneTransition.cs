using System.Collections;

public interface ISceneTransition
{
    IEnumerator Show();
    IEnumerator Hide();
}
