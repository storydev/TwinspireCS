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

        private List<Wave> waves;
        private List<Music> music;
        private List<Font> fonts;
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
        }

        public void AddWave(Wave wave)
        {
            waves.Add(wave);
        }

        public void AddMusic(Music music)
        {
            this.music.Add(music);
        }

        public void AddFont(Font font)
        {
            fonts.Add(font);
        }

        public void AddImage(Image image)
        {
            images.Add(image);
        }

    }
}
