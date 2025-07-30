using System;

public class AnimationNotFoundException : Exception
{
    public AnimationNotFoundException()
    {
    }

    public AnimationNotFoundException(string message) : base(message)
    {
    }

    public AnimationNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}