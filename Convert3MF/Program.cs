using System;
using System.Linq;
using System.IO;
using Lib3MF;
using Lib3MF.Internal;

namespace Convert3MF
{ 
    class Program
    {

        public const int LIB3MF_OK = 0;
        public const uint LIB3MF_FAIL = 0x80004005;
        public const uint LIB3MF_POINTER = 0x80004003;
        public const uint LIB3MF_INVALIDARG = 0x80070057;

        static void Main(string[] args)
        {
            string fileName = new string(new char[999]);
            string outputName = new string(new char[999]);

            if (args.Length < 1)
            {
                Console.WriteLine("Input file Name ");
                fileName = Console.ReadLine();
            }
            else
            {
                fileName = args[1];
            }

            outputName = fileName.Substring(0, fileName.Length - fileName.IndexOf('.'));
            outputName += ".3mf";

            Console.WriteLine("Generating 3MF Model... " + outputName);
            Console.WriteLine("\n");

            int hResult;
            uint nInterfaceVersionMajor;
            uint nInterfaceVersionMinor;
            uint nInterfaceVersionMicro;
            UInt32 nErrorMessage;
            string pszErrorMessage;

            IntPtr handler;

            CModel pModel;
            IntPtr p3MFWriter;
            IntPtr pBuildItem;
            IntPtr pPropertyHandler;
            IntPtr pDefaultPropertyHandler;

            // Create Model Instance
            pModel = Wrapper.CreateModel();

            // Add Color Group
            CColorGroup aColorGroup = pModel.AddColorGroup();

            StreamReader reader = new StreamReader(File.OpenRead(fileName));
            if (reader == null)
            {
                Console.Write("Unable to open file");
                return;
            }

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
                // Add Color to colorgroup
                sColor aColor;
                strColor = reader.ReadLine();
                stringValues = strColor.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                aColor.Red = Byte.Parse(stringValues[1]);
                aColor.Green = Byte.Parse(stringValues[2]);
                aColor.Blue = Byte.Parse(stringValues[3]);
                aColor.Alpha = 255;                
                aColorGroup.AddColor(aColor);

                // Get vertext count and triangle count
                int verticiesCount, triangleCount;

                strVertex = reader.ReadLine();
                stringValues = strVertex.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                verticiesCount = Int32.Parse(stringValues[1]);

                strTriangle = reader.ReadLine();
                stringValues = strTriangle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                triangleCount = Int32.Parse(stringValues[1]);

                //##############################################################################	Mesh Object	Start
                // Create Mesh Object
                CMeshObject pMeshObject = pModel.AddMeshObject();

                pMeshObject.SetName("Colored Box");
                
                // Create mesh structure of a cube

                //MODELMESHVERTEX* pVertices = new MODELMESHVERTEX[verticiesCount];
                //MODELMESHTRIANGLE* pTriangles = new MODELMESHTRIANGLE[trianglesCount];
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
                    pMeshObject.AddVertex(position);
                    aVertices[i] = position;
                }

                sTriangle[] aIndices = new sTriangle[triangleCount];
                for (int i = 0; i < triangleCount; i++)
                {
                    sTriangle triangle;
                    triangle.Indices = new UInt32[3];
                    strTemp = reader.ReadLine();
                    stringValues = strTemp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    triangle.Indices[0] = UInt32.Parse(stringValues[0]);
                    triangle.Indices[1] = UInt32.Parse(stringValues[1]);
                    triangle.Indices[2] = UInt32.Parse(stringValues[2]);
                    pMeshObject.AddTriangle(triangle);
                    aIndices[i] = triangle;
                }

                //pMeshObject.GetVertices(out aVertices);
                //pMeshObject.GetTriangleIndices(out aIndices);

                //pMeshObject.SetGeometry(aVertices, aIndices);

                //if (hResult != LIB3MF_OK)
                //{
                //    std::cout << "could not set mesh geometry: " << std::hex << hResult << std::endl;
                //    lib3mf_getlasterror(pMeshObject, &nErrorMessage, &pszErrorMessage);
                //    std::cout << "error #" << std::hex << nErrorMessage << ": " << pszErrorMessage << std::endl;
                //    lib3mf_release(pMeshObject);
                //    lib3mf_release(pModel);
                //    return -1;
                //}

                //MODELMESHCOLOR_SRGB sColorRed = fnCreateColor(colorR, colorG, colorB);

                //hResult = lib3mf_object_createdefaultpropertyhandler(pMeshObject, &pDefaultPropertyHandler);
                //if (hResult != LIB3MF_OK)
                //{
                //    std::cout << "could not create default property handler: " << std::hex << hResult << std::endl;
                //    lib3mf_getlasterror(pMeshObject, &nErrorMessage, &pszErrorMessage);
                //    std::cout << "error #" << std::hex << nErrorMessage << ": " << pszErrorMessage << std::endl;
                //    lib3mf_release(pMeshObject);
                //    lib3mf_release(pModel);
                //    return -1;
                //}

                //lib3mf_defaultpropertyhandler_setcolor(pDefaultPropertyHandler, &sColorRed);
                //lib3mf_release(pDefaultPropertyHandler);

                //Add Build Item for Mesh
                sTransform aTransform;
                CBuildItem aBuildItem = pModel.AddBuildItem(pMeshObject, aTransform);

                //if (hResult != LIB3MF_OK)
                //{
                //    std::cout << "could not create build item: " << std::hex << hResult << std::endl;
                //    lib3mf_getlasterror(pModel, &nErrorMessage, &pszErrorMessage);
                //    std::cout << "error #" << std::hex << nErrorMessage << ": " << pszErrorMessage << std::endl;
                //    lib3mf_release(pMeshObject);
                //    lib3mf_release(pModel);
                //    return -1;
                //}

                // Output cube as STL and 3MF
                Wrapper.Release(pMeshObject);
                //Wrapper.Release(aBuildItem);
                //############################################################################		Mesh Object end
            }
            reader.Close();

            // Create Model Writer
            CWriter aWriter = pModel.QueryWriter("3mf");
            
            // Export Model into File
            Console.WriteLine("Writing ...");
            aWriter.WriteToFile(outputName);

            // Release Model Writer
            Wrapper.Release(aWriter);

            // Release Model
            Wrapper.Release(pModel);

            Console.WriteLine("Done");
        }
    }
}
