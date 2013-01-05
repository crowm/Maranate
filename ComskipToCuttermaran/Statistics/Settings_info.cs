using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComskipToCuttermaran.Statistics
{
    public class Settings_info
    {
        public int LogoDetectSearch_StartPosition_Percentage = 10;
        public int LogoDetectSearch_EndPosition_Percentage = 90;

        public int BrightnessThreshold = -1;
        public double DetectBrightnessSafetyBufferPercent = 0.2;
        public int DetectBrightnessSafetyBufferOffset = 0;

        public int UniformThreshold = -1;
        public double DetectUniformSafetyBufferPercent = 0.0;
        public int DetectUniformSafetyBufferOffset = 20;

        public int SoundThreshold = -1;
        public double DetectSoundSafetyBufferPercent = 0.0;
        public int DetectSoundSafetyBufferOffset = 20;

        public double ScoreAboveAverageBrightnessThreshold = 0.0;
        public double ScoreAboveAverageBrightnessMultiplier = -1.5;
        public double ScoreBelowAverageBrightnessThreshold = 0.0;
        public double ScoreBelowAverageBrightnessMultiplier = 1.5;

        public double ScoreAboveAverageLogoThreshold = 0.0;
        public double ScoreAboveAverageLogoMultiplier = 1.0;
        public double ScoreBelowAverageLogoThreshold = 0.0;
        public double ScoreBelowAverageLogoMultiplier = -0.5;

        public double ScoreAboveAverageUniformThreshold = 0.0;
        public double ScoreAboveAverageUniformMultiplier = -0.5;
        public double ScoreBelowAverageUniformThreshold = 0.0;
        public double ScoreBelowAverageUniformMultiplier = 0.4;

        public double ScoreAboveAverageLogSoundThreshold = 0.1;
        public double ScoreAboveAverageLogSoundMultiplier = -1.0;
        public double ScoreBelowAverageLogSoundThreshold = -0.2;
        public double ScoreBelowAverageLogSoundMultiplier = -0.5;
    }
}
