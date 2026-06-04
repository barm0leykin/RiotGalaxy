using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace RiotGalaxy.Managers
{
    /// <summary>
    /// Менеджер аудио. Загружает и проигрывает звуковые эффекты (SFX).
    /// Аналог использования CCAudioEngine.PlayEffect из CocosSharp.
    /// </summary>
    public class AudioManager
    {
        private static AudioManager _instance;
        public static AudioManager Instance => _instance ??= new AudioManager();

        // Загруженные звуковые эффекты по короткому имени ("fire1", "explode1")
        private readonly Dictionary<string, SoundEffect> _effects = new Dictionary<string, SoundEffect>();

        // Громкость эффектов (в оригинале CCAudioEngine.EffectsVolume = 0.1f)
        public float EffectsVolume { get; set; } = 0.1f;

        private AudioManager() { }

        /// <summary>
        /// Загрузка звуковых ассетов из Content Pipeline.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            TryLoadEffect(content, "fire1", "Sounds/fire1");
            TryLoadEffect(content, "explode1", "Sounds/explode1");
        }

        private void TryLoadEffect(ContentManager content, string key, string asset)
        {
            try
            {
                _effects[key] = content.Load<SoundEffect>(asset);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Failed to load SoundEffect '{asset}': {ex.Message} ===");
            }
        }

        /// <summary>
        /// Проиграть звуковой эффект по имени (аналог CCAudioEngine.PlayEffect).
        /// </summary>
        public void PlayEffect(string key)
        {
            if (_effects.TryGetValue(key, out var effect))
            {
                try
                {
                    effect.Play(EffectsVolume, 0f, 0f);
                }
                catch (Exception ex)
                {
                    // Например, NoAudioHardwareException на машинах без звука
                    Console.WriteLine($"=== Failed to play effect '{key}': {ex.Message} ===");
                }
            }
        }
    }
}
