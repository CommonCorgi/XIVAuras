﻿using System.Numerics;

namespace XIVAuras.Config
{
    public interface IConfigPage
    {
        string Name { get; }

        void DrawConfig(Vector2 size, float padX, float padY);
    }
}
