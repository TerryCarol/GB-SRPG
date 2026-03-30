namespace Command
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CommandInvoker : MonoBehaviour
    {
        // 싱글톤 인스턴스
        private static CommandInvoker instance;
        // 외부 접근 스태틱 프로퍼티
        public static CommandInvoker Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject invokerObject = new GameObject("CommandInvoker");
                    instance = invokerObject.AddComponent<CommandInvoker>();
                    DontDestroyOnLoad(invokerObject);
                }
                return instance;
            }
        }

        // Command 큐 (선입선출)
        private Queue<ICommand> commandQueue = new Queue<ICommand>();

        // 명령 큐에 추가
        public void SetCommand(ICommand command)
        {
            commandQueue.Enqueue(command);
        }

        // 명령 큐 초기화
        public void ClearCommand()
        {
            commandQueue.Clear();
        }

        // 명령 실행
        private void Update()
        {
            if (commandQueue.Count > 0)
            {
                ICommand currentCommand = commandQueue.Dequeue();
                currentCommand.Execute();
            }
        }
    }
}
