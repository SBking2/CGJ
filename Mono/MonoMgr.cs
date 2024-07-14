using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace ProjectBase
{
    /// <summary>
    /// ��û�м̳�MonoBehavior�������ʵ�������
    /// </summary>
    public class MonoMgr : BaseManager<MonoMgr>
    {
        private MonoController m_controller;

        public MonoMgr()
        {
            GameObject obj = new GameObject("MonoController");
            m_controller = obj.AddComponent<MonoController>();
        }

        public void AddUpdateListener(UnityAction action)
        {
            m_controller.AddUpdateListener(action);
        }

        public void RemoveUpdateListener(UnityAction action)
        {
            m_controller.RemoveUpdateListener(action);
        }

        public Coroutine StartRoutine(IEnumerator routine)
        {
            return m_controller.StartRoutine(routine);
        }

        public Coroutine StartRoutine(string methodName, [DefaultValue("null")] object value)
        {
            return m_controller.StartRoutine(methodName, value);
        }

        public Coroutine StartRoutine(string methodName)
        {
            return m_controller.StartRoutine(methodName);
        }
    }
}
