using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public class ResourceResults
    {

        private List<string> waveNames;
        private List<Wave> waves;
        private List<string> musicNames;
        private List<Music> music;
        private List<string> fontNames;
        private List<Font> fonts;
        private List<string> imageNames;
        private List<Image> images;

        public IEnumerable<Wave> Waves { get => waves; }

        public IEnumerable<Music> Music { get => music; }

        public IEnumerable<Font> Fonts { get => fonts; }

        public IEnumerable<Image> Images { get => images; }

        public ResourceResults()
        {
            waves = new List<Wave>();
            music = new List<Music>();
            fonts = new List<Font>();
            images = new List<Image>();

            waveNames = new List<string>();
            musicNames = new List<string>();
            fontNames = new List<string>();
            imageNames = new List<string>();
        }

        public void AddWave(Wave wave, string identifier)
        {
            waves.Add(wave);
            waveNames.Add(identifier);
        }

        public void AddMusic(Music music, string identifier)
        {
            this.music.Add(music);
            musicNames.Add(identifier);
        }

        public void AddFont(Font font, string identifier)
        {
            fonts.Add(font);
            fontNames.Add(identifier);
        }

        public void AddImage(Image image, string identifier)
        {
            images.Add(image);
            imageNames.Add(identifier);
        }

        public Font GetFont(string identifier)
        {
            for (int i = 0; i < fontNames.Count; i++)
            {
                if (fontNames[i] == identifier)
                    return fonts[i];
            }


        }

    }
}
