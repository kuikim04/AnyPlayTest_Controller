using FishNet;
using FishNet.Transporting.Tugboat;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClientSeverManager : MonoBehaviour
{
    [SerializeField] private ConnectionType _connectionType;

    private Tugboat _tugboat;

    private void Awake()
    {
        // Check if Tugboat component exists on this GameObject
        if (TryGetComponent(out Tugboat tug))
            _tugboat = tug;
        else
        {
            Debug.LogError("Tugboat not found", gameObject);
            return;
        }

        // In the Unity Editor, check the connection type and start connections accordingly
#if UNITY_EDITOR
        if (_connectionType == ConnectionType.Host)
        {
            // If running as a clone, start connection without hosting
            if (ParrelSync.ClonesManager.IsClone())
            {
                _tugboat.StartConnection(false);
            }
            else
            {
                // Start host connection and client connection
                _tugboat.StartConnection(true);
                _tugboat.StartConnection(false);
            }

            return;
        }

        // For non-host connections in the Unity Editor, start client connection
        _tugboat.StartConnection(false);
#endif

        // For non-Unity Editor builds, start client connection
#if !UNITY_EDITOR
        _tugboat.StartConnection(false);
#endif
    }

    private void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        // If the client is in the process of stopping, exit play mode in the Unity Editor
#if UNITY_EDITOR
        if (args.ConnectionState == LocalConnectionState.Stopping)
            EditorApplication.isPlaying = false;
#endif
    }

    public enum ConnectionType
    {
        Host,
        Client
    }
}
