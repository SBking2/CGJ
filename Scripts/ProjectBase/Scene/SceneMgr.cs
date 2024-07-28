using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ProjectBase
{
    public class SceneMgr : BaseManager<SceneMgr>
    {
        /// <summary>
        /// ͬ�����س���
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadScene(string sceneName, UnityAction action)
        {
            SceneManager.LoadScene(sceneName);

            action();
        }

        /// <summary>
        /// �첽���س���������Э��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadSceneAsyn(string sceneName, UnityAction action)
        {
            MonoMgr.GetInstance().StartRoutine(ReallyLoadSceneAsyn(sceneName, action));
        }

        private IEnumerator ReallyLoadSceneAsyn(string sceneName, UnityAction action)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

            while(!ao.isDone)
            {
                //TODO:���͸��¼����ĳ����ļ��ؽ���
                yield return ao.progress;
            }

            action();
        }
    }
}
