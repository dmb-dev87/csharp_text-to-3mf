using System;
using System.Linq;
using System.IO;
using Lib3MF;
using System.Runtime.InteropServices;

namespace Convert3MF
{ 
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = new string(new char[999]);

            if (args.Length < 1)
            {
                Console.WriteLine("Input file Name: ");
                fileName = Console.ReadLine();
            }
            else
            {
                fileName = args[0];
            }

            string str_pathSlash = "\\";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                str_pathSlash = "/";
            }
            else
            {
                str_pathSlash = "\\";
            }

            string inputPath = Path.GetFullPath(fileName);

            string outputPath = inputPath.Substring(0, inputPath.LastIndexOf(str_pathSlash));
            string outputName = inputPath.Substring(inputPath.LastIndexOf(str_pathSlash) +1, inputPath.LastIndexOf('.') - inputPath.LastIndexOf(str_pathSlash) - 1);
            outputPath = outputPath + str_pathSlash + outputName + ".3mf";

            Console.WriteLine("Generating 3MF Model: " + outputPath);

            StreamReader reader;

            try
            {
                reader = new StreamReader(File.OpenRead(inputPath));
                if (reader == null)
                {
                    Console.WriteLine("Unable to open file");
                    return;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Cannot find the file.");
                return;
            }

            Console.WriteLine("Create model...");

            CModel aModel;
            CWriter a3MFWriter;
            CBuildItem aBuildItem;
            CColorGroup aColorGroup;


            // Create Model Instance
            aModel = Wrapper.CreateModel();

            // Add Color Group
            aColorGroup = aModel.AddColorGroup();

            string strModelCount;
            string strColor;
            string strVertex;
            string strTriangle;
            string strTemp;

            int modelCount;
            strModelCount = reader.ReadLine();
            string[] stringValues = strModelCount.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            modelCount = Int32.Parse(stringValues[1]);

            for (int modelId = 0; modelId < modelCount; modelId++)
            {
                //##############################################################################	Mesh Object	Start
                // Create Mesh Object
                CMeshObject aMeshObject = aModel.AddMeshObject();
                uint aPropertyId;

                // Add Color to colorgroup
                sColor aColor;
                strColor = reader.ReadLine();
                stringValues = strColor.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                aColor.Red = Byte.Parse(stringValues[1]);
                aColor.Green = Byte.Parse(stringValues[2]);
                aColor.Blue = Byte.Parse(stringValues[3]);
                aColor.Alpha = 255;

                aMeshObject.SetName("Colored Box");

                // Get vertext count and triangle count
                int verticiesCount, triangleCount;

                strVertex = reader.ReadLine();
                stringValues = strVertex.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                verticiesCount = Int32.Parse(stringValues[1]);

                strTriangle = reader.ReadLine();
                stringValues = strTriangle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                triangleCount = Int32.Parse(stringValues[1]);

                // Create mesh structure of a cube
                sPosition[] aVertices = new sPosition[verticiesCount];
                for (int i = 0; i < verticiesCount; i++)
                {
                    sPosition position;
                    position.Coordinates = new Single[3];
                    strTemp = reader.ReadLine();
                    stringValues = strTemp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    position.Coordinates[0] = Single.Parse(stringValues[0]);
                    position.Coordinates[1] = Single.Parse(stringValues[1]);
                    position.Coordinates[2] = Single.Parse(stringValues[2]);
                    //aMeshObject.AddVertex(position);
                    aVertices[i] = position;
                }

                sTriangle[] aIndices = new sTriangle[triangleCount];
                for (int j = 0; j < triangleCount; j++)
                {
                    sTriangle triangle;
                    triangle.Indices = new UInt32[3];
                    strTemp = reader.ReadLine();
                    stringValues = strTemp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    triangle.Indices[0] = UInt32.Parse(stringValues[0]);
                    triangle.Indices[1] = UInt32.Parse(stringValues[1]);
                    triangle.Indices[2] = UInt32.Parse(stringValues[2]);
                    //aMeshObject.AddTriangle(triangle);
                    aIndices[j] = triangle;
                }                

                aMeshObject.SetGeometry(aVertices, aIndices);

                aPropertyId = aColorGroup.AddColor(aColor);

                aMeshObject.SetObjectLevelProperty(aColorGroup.GetUniqueResourceID(), aPropertyId);

                // Set initial transform
                sTransform aTransform = Wrapper.GetIdentityTransform();

                //Add Build Item for Mesh
                aBuildItem = aModel.AddBuildItem(aMeshObject, aTransform);

                // Output cube as STL and 3MF
                Wrapper.Release(aMeshObject);
                Wrapper.Release(aBuildItem);
                //############################################################################		Mesh Object end
            }
            reader.Close();

            // Create Model Writer
            a3MFWriter = aModel.QueryWriter("3mf");
            
            // Export Model into File
            Console.WriteLine("Writing to file...");
            a3MFWriter.WriteToFile(outputPath);

            // Release Model Writer
            Wrapper.Release(a3MFWriter);

            // Release Model
            Wrapper.Release(aModel);

            Console.WriteLine("Done.");
        }
    }
}
