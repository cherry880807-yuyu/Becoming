using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboInputBuffer
{
    private bool _hasNext;
    private float _expireTime;
    private float _bufferDuration;
    public ComboInputBuffer(float bufferDuration = 0.4f)
    {
        _bufferDuration = bufferDuration;
    }

    public void Enqueue()
    {
        _hasNext = true;
        _expireTime = Time.time + _bufferDuration;
        Debug.Log("Input Buffered");
    }

    public bool Dequeue()
    {
        if (!_hasNext)
        {
            //Debug.Log("No Buffered Input");
            return false;
        }

        if (Time.time > _expireTime)
        {
            //Debug.Log("Buffered Input Expired");
            _hasNext = false;
            return false;
        }
        
        //Debug.Log("Buffered Input Consumed");
        _hasNext = false;
        return true;
    }

    public void Clear() => _hasNext = false;
}