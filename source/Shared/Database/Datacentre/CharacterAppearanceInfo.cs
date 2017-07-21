using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Shared.Database.Datacentre
{
    public struct CharacterAppearanceInfo
    {
        public byte Race
        {
            get => Data[0];
            private set => Data[0] = value;
        }

        public byte Sex
        {
            get => Data[1];
            private set => Data[1] = value;
        }

        public byte Height
        {
            get => Data[3];
            private set => Data[3] = value;
        }

        public byte Clan
        {
            get => Data[4];
            private set => Data[4] = value;
        }

        public byte Face
        {
            get => Data[5];
            private set => Data[5] = value;
        }

        public byte HairStyle
        {
            get => Data[6];
            private set => Data[6] = value;
        }

        public byte HairColourHl
        {
            get => Data[7];
            private set => Data[7] = value;
        }

        public byte SkinColour
        {
            get => Data[8];
            private set => Data[8] = value;
        }

        public byte EyeColourOdd
        {
            get => Data[9];
            private set => Data[9] = value;
        }

        public byte HairColour
        {
            get => Data[10];
            private set => Data[10] = value;
        }

        public byte FacialFeatures
        {
            get => Data[12];
            private set => Data[12] = value;
        }

        public byte TattooColour
        {
            get => Data[13];
            private set => Data[13] = value;
        }

        public byte Eyebrows
        {
            get => Data[14];
            private set => Data[14] = value;
        }

        public byte EyeColour
        {
            get => Data[15];
            private set => Data[15] = value;
        }

        public byte Eye
        {
            get => Data[16];
            private set => Data[16] = value;
        }

        public byte Nose
        {
            get => Data[17];
            private set => Data[17] = value;
        }

        public byte Jaw
        {
            get => Data[18];
            private set => Data[18] = value;
        }

        public byte Mouth
        {
            get => Data[19];
            private set => Data[19] = value;
        }

        public byte LipColour
        {
            get => Data[20];
            private set => Data[20] = value;
        }

        public byte TailLength
        {
            get => Data[21];
            private set => Data[21] = value;
        }

        public byte TailShape
        {
            get => Data[22];
            private set => Data[22] = value;
        }

        public byte BustSize
        {
            get => Data[23];
            private set => Data[23] = value;
        }

        public byte FacePaint
        {
            get => Data[24];
            private set => Data[24] = value;
        }

        public byte FacePaintColour
        {
            get => Data[25];
            private set => Data[25] = value;
        }

        public JArray Array => JArray.FromObject(new List<byte>(Data));
        public byte[] Data { get; }

        public CharacterAppearanceInfo(JArray array)
        {
            Data = array.Values<byte>().ToArray();
        }

        public CharacterAppearanceInfo(DataRow row)
        {
            Data            = new byte[26];
            Race            = row.Read<byte>("race");
            Sex             = row.Read<byte>("sex");
            Height          = row.Read<byte>("height");
            Clan            = row.Read<byte>("clan");
            Face            = row.Read<byte>("face");
            HairStyle       = row.Read<byte>("hairStyle");
            HairColourHl    = row.Read<byte>("hairColourHighlights");
            SkinColour      = row.Read<byte>("skinColour");
            EyeColourOdd    = row.Read<byte>("eyeColourOdd");
            HairColour      = row.Read<byte>("hairColour");
            FacialFeatures  = row.Read<byte>("facialFeatures");
            TattooColour    = row.Read<byte>("tattooColour");
            Eyebrows        = row.Read<byte>("eyebrows");
            EyeColour       = row.Read<byte>("eyeColour");
            Eye             = row.Read<byte>("eye");
            Nose            = row.Read<byte>("nose");
            Jaw             = row.Read<byte>("jaw");
            Mouth           = row.Read<byte>("mouth");
            LipColour       = row.Read<byte>("lipColour");
            TailLength      = row.Read<byte>("tailLength");
            TailShape       = row.Read<byte>("tailShape");
            BustSize        = row.Read<byte>("bustSize");
            FacePaint       = row.Read<byte>("facePaint");
            FacePaintColour = row.Read<byte>("facePaintColour");
        }

        public bool Verify()
        {
            // TODO: verify data
            return true;
        }

        public void SaveToDatabase(ulong id, DatabaseTransaction transaction)
        {
            transaction.AddPreparedStatement(DataCentreDatabase.DataCentrePreparedStatement.CharacterAppearanceInsert,
                id, Race, Sex, Height, Clan, BustSize, SkinColour, TailShape, TailLength, HairStyle, HairColour, HairColourHl, Face, Jaw,
                Eye, EyeColour, EyeColourOdd, Eyebrows, Nose, Mouth, LipColour, FacialFeatures, TattooColour, FacePaint, FacePaintColour);
        }
    }
}
