using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TwinspireCS.Audio
{
    public class Player
    {

        private int numSoundsReserved;
        private int maxSoundChannels;
        private bool crossFadeMusic;

        private int startMusicPlayingIndex;
        private int startVoicePlayingIndex;
        private int startAmbiencePlayingIndex;
        private List<bool> isPlaying;

        public float MusicVolume;
        public float SoundEffectVolume;

        public Dictionary<string, Sound> SoundEffects;
        public SoundChannel[] Ambience;
        public SoundChannel[] Voice;
        public SoundChannel[] Music;

        /// <summary>
        /// The audio player has a multitude of functions to play, edit
        /// and store different sounds, play them as playlists, cross-fade,
        /// and more.
        /// </summary>
        public Player()
        {
            SoundEffects = new Dictionary<string, Sound>();
            Ambience = Array.Empty<SoundChannel>();
            Voice = Array.Empty<SoundChannel>();
            Music = Array.Empty<SoundChannel>();

            isPlaying = new List<bool>();

            MusicVolume = 1.0f;
            SoundEffectVolume = 0.5f;
        }

        /// <summary>
        /// Initialise the player with the given number of maximum sound channels. This must be called
        /// before using the player.
        /// </summary>
        /// <param name="maxSoundChannels">The maximum number of sound channels to store.</param>
        /// <param name="numSoundsReserved">The number of sound channels to reserve for sound effects.</param>
        /// <param name="allowMusicCrossFade">Allow music to cross-fade. Requires two music channels to function.</param>
        /// <param name="multipleVoiceTracks">Allow for multiple voice tracks. The number of voice tracks given is determined by how many sound channels remain once everything else is allocated.</param>
        /// <exception cref="Exception">If the number of maximum sound channels is not divisible by two, or the number of 
        /// sound channels for sound effects reserved is greater than the given maximum, an exception is thrown.
        /// </exception>
        public void Init(int maxSoundChannels, int numSoundsReserved, bool allowMusicCrossFade = false, bool multipleVoiceTracks = false)
        {
            this.maxSoundChannels = maxSoundChannels;
            this.numSoundsReserved = numSoundsReserved;

            crossFadeMusic = allowMusicCrossFade;

            if (maxSoundChannels % 2 != 0)
            {
                throw new Exception("maxSoundChannels must be divisible by two.");
            }

            if (numSoundsReserved > maxSoundChannels)
            {
                throw new Exception("The number of sounds being reserved is greater than the maximum sound channels.");
            }

            var musicChannels = 1;
            if (allowMusicCrossFade)
                musicChannels = 2;

            startMusicPlayingIndex = 0;

            Music = new SoundChannel[musicChannels];

            startVoicePlayingIndex = musicChannels;
            var remaining = maxSoundChannels - musicChannels;
            if (multipleVoiceTracks)
            {
                var half = remaining / 2;
                startAmbiencePlayingIndex = startVoicePlayingIndex + half;
                Voice = new SoundChannel[half];
                Ambience = new SoundChannel[remaining - half];
            }
            else
            {
                Voice = new SoundChannel[1];
                startAmbiencePlayingIndex = startVoicePlayingIndex + 1;
                Ambience = new SoundChannel[remaining - 1];
            }

            for (int i = 0; i < maxSoundChannels - numSoundsReserved; i++)
                isPlaying.Add(false);
        }

        /// <summary>
        /// Updates play states.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < Music.Length; i++)
            {
                var musicChannel = Music[i];
                if (string.IsNullOrEmpty(musicChannel.AudioName))
                    continue;

                var music = Application.Instance.ResourceManager.GetMusic(musicChannel.AudioName);
                Raylib.SetMusicVolume(music, musicChannel.Volume);
                Raylib.SetMusicPan(music, musicChannel.Pan);
                Raylib.SetMusicPitch(music, musicChannel.Pitch);

                Raylib.UpdateMusicStream(music);
            }
        }

        public int PlayMusic(string identifier)
        {
            var isAnyMusicPlaying = false;
            for (int i = startMusicPlayingIndex; i < startVoicePlayingIndex; i++)
            {
                if (isPlaying[i])
                    isAnyMusicPlaying = true;
            }

            if (!isAnyMusicPlaying || !crossFadeMusic)
            {
                var playingMusicChannel = 0;
                for (int i = 0; i < Music.Length; i++)
                {
                    if (Equals(Music[i], new SoundChannel()))
                    {
                        Music[i].AudioName = identifier;
                        Music[i].Volume = MusicVolume;
                        Music[i].Pan = 0.5f;
                        Music[i].Pitch = 1.0f;

                        var music = Application.Instance.ResourceManager.GetMusic(identifier);
                        Raylib.PlayMusicStream(music);
                        isPlaying[startMusicPlayingIndex + i] = true;
                        playingMusicChannel = i;
                        break;
                    }
                }
                return playingMusicChannel;
            }
            else
            {
                if (!crossFadeMusic)
                {

                    return 0;
                }
                    
                // TODO: Implement cross-fade

            }

            return 0;
        }

        public void PlaySound(string identifier)
        {
            if (SoundEffects.ContainsKey(identifier))
            {
                var sound = SoundEffects[identifier];
                Raylib.PlaySound(sound);
            }
            else
            {
                if (SoundEffects.Count >= numSoundsReserved)
                    return;

                var wave = Application.Instance.ResourceManager.GetWave(identifier);
                var sound = Raylib.LoadSoundFromWave(wave);
                SoundEffects.Add(identifier, sound);

                Raylib.SetSoundVolume(sound, SoundEffectVolume);
                Raylib.SetSoundPitch(sound, 1.0f);
                Raylib.SetSoundPan(sound, 0.5f);

                Raylib.PlaySound(sound);
            }
        }

        public void UpdateSound(string identifier, float pitch, float pan)
        {
            if (SoundEffects.ContainsKey(identifier))
            {
                if (!Raylib.IsSoundPlaying(SoundEffects[identifier]))
                {
                    SoundEffects.Remove(identifier);
                    return;
                }

                Raylib.SetSoundPitch(SoundEffects[identifier], pitch);
                Raylib.SetSoundPan(SoundEffects[identifier], pan);
            }
        }


        private static Player? _instance;
        public static Player Instance => _instance ??= new Player();

    }
}
