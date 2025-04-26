using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Meadows.Utility {
    public static class Sound {
        private class Pool {
            public List<SoundEffectInstance> Instances = new List<SoundEffectInstance>();
            public SoundEffect Sound;
            public int MaxInstances;

            public Pool(SoundEffect sound, int max) {
                MaxInstances = max;
                Sound = sound;
            }
        }

        private static Dictionary<String, Pool> sounds = new Dictionary<string, Pool>();
        public static float MasterVolume { get; set;  } = 1.0f;

        public static void Load(ContentManager content, String name, String path, int max = 10) {
            if (!sounds.ContainsKey(name)) {
                var sound = content.Load<SoundEffect>(path);
                sounds.Add(name, new Pool(sound, max));
            }
        }

        private static SoundEffectInstance GetAvailableInstance(Pool pool) {
            foreach (var instance in pool.Instances) {
                if (instance.State == SoundState.Stopped)
                    return instance;
            }

            return null;
        }

        private static double LastTime = 0;
        public static void Play(GameTime dt, String name, float cooldown = 0.0f, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f) {
            if (!sounds.ContainsKey(name))
                return;

            if ((cooldown > 0.0f) && (dt.TotalGameTime.TotalSeconds - LastTime < cooldown))
                return;

            var pool = sounds[name];
            var instance = GetAvailableInstance(pool);

            if (instance == null) {
                if (pool.Instances.Count < pool.MaxInstances) {
                    instance = pool.Sound.CreateInstance();
                    pool.Instances.Add(instance);
                } else {
                    return;
                }
            }

            LastTime = dt.TotalGameTime.TotalSeconds;
            instance.Volume = volume * MasterVolume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.Play();
        }

        public static void SetVolume(float volume) {
            MasterVolume = Microsoft.Xna.Framework.MathHelper.Clamp(volume, 0f, 1f);
        }
    }
}
