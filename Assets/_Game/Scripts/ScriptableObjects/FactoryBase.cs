using UnityEngine.SceneManagement;
using UnityEngine;

public abstract class FactoryBase : ScriptableObject
{
    private Scene _scene;

    protected T CreateInstance<T>(T prefab) where T : MonoBehaviour
    {
        if (!this._scene.isLoaded)
        {
            if (Application.isEditor)
            {
                this._scene = SceneManager.GetSceneByName(name);
                if (!this._scene.isLoaded)
                {
                    this._scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                this._scene = SceneManager.CreateScene(name);
            }
        }

        T instance = Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(instance.gameObject, this._scene);

        return instance;
    }
}
