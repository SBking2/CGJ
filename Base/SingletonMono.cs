using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;

        /// <summary>
        /// Instanceֻ��ָ�����Awake�Ľű���������U3D��ֻ�ܹ���һ��
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            return m_instance;
        }

        protected virtual void Awake()
        {
            m_instance = this as T;
        }
    }
}
