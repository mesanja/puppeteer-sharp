﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PuppeteerSharp
{
    public class Session : IDisposable
    {
        public Session(Connection connection, string targetId, int sessionId)
        {
            Connection = connection;
            TargetId = targetId;
            SessionId = sessionId;
        }

        #region Private Memebers
        private int _lastId = 0;
        #endregion

        #region Properties
        public string TargetId { get; private set; }
        public int SessionId { get; private set; }
        public Connection Connection { get; private set; }
        public event EventHandler<MessageEventArgs> MessageReceived;
        #endregion

        #region Public Methods

        public async Task<T> SendAsync<T>(string method, params object[] args)
        {
            return Convert.ChangeType(await SendAsync(method, args), typeof(T));
        }

        public async Task<dynamic> SendAsync(string method, params object[] args)
        {
            if (Connection == null)
            {
                throw new Exception($"Protocol error (${method}): Session closed. Most likely the page has been closed.");
            }
            int id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>(){
                {"id", id},
                {"method", method},
                {"params", args}
            });

            return await Connection.SendAsync("Target.sendMessageToTarget", new Dictionary<string, object>() {
                {"sessionId", SessionId},
                {"message", message}
            });

        }
        #endregion

        #region Private Mathods

        public void Dispose()
        {
            Connection.SendAsync("Target.closeTarget", new Dictionary<string, object>() {
                {"targetId", TargetId}
            }).GetAwaiter().GetResult();
        }

        internal void OnMessage(dynamic message)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
