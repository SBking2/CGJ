using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectBase
{

    public interface IEventInfo
    {

    }

    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }

    public class EventInfo : IEventInfo
    {
        public UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }

    public class EventCenter : BaseManager<EventCenter>
    {
        private Dictionary<string, IEventInfo> m_eventDic = new Dictionary<string, IEventInfo>();

        /// <summary>
        /// ���Listener��name���¼���
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void AddListener<T>(string eventName, UnityAction<T> action)
        {
            if(m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo<T>).actions += action;
            }else
            {
                m_eventDic.Add(eventName, new EventInfo<T>(action));
            }
        }

        public void AddListener(string eventName, UnityAction action)
        {
            if (m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo).actions += action;
            }
            else
            {
                m_eventDic.Add(eventName, new EventInfo(action));
            }
        }

        /// <summary>
        /// ����name�Ƴ������¼�
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void RemoveListener<T>(string eventName, UnityAction<T> action)
        {
            if(m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo<T>).actions -= action;
            }
        }

        public void RemoveListener(string eventName, UnityAction action)
        {
            if (m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo).actions -= action;
            }
        }

        /// <summary>
        /// ����name�����¼�
        /// </summary>
        /// <param name="eventName"></param>
        public void EventTrigger<T>(string eventName, T info)
        {
            if(m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo<T>).actions.Invoke(info);
            }
        }

        public void EventTrigger(string eventName)
        {
            if (m_eventDic.ContainsKey(eventName))
            {
                (m_eventDic[eventName] as EventInfo).actions.Invoke();
            }
        }

        /// <summary>
        /// ����¼����İ󶨵��¼���һ�����л�����ʱʹ��
        /// </summary>
        public void Clear()
        {
            m_eventDic.Clear();
        }
    }
}
