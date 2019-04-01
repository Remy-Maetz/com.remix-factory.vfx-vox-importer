using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.VFX.Utils;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using com.RemixFactory.VFXVox;

// File format documentation here : https://github.com/ephtracy/voxel-model/blob/master/MagicaVoxel-file-format-vox.txt

namespace com.RemixFactoryEditor.VFXVox
{

	[ScriptedImporter(1, "vox")]
	public class VoxImporter : ScriptedImporter
	{
		public int modelCount = 1;

		public Color32[] palette;
		public Texture2D paletteTexture;
		
		uint[] defaultPalette = {
			0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
			0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
			0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
			0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
			0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
			0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
			0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
			0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
			0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
			0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
			0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
			0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
			0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
			0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
			0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
			0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
		};

		//[HideInInspector]
		public List<Model> models;

		Vox_Transform topmostTransform;
		
		List<Vox_Transform> vox_Transforms = new List<Vox_Transform>();
		List<Vox_Group> vox_Groups = new List<Vox_Group>();
		List<Vox_Shape> vox_Shapes = new List<Vox_Shape>();
		List<Vox_Layer> vox_Layers = new List<Vox_Layer>();

		[HideInInspector]
		public List<Vox_Material> materials;
		
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var fileStream = System.IO.File.OpenRead(ctx.assetPath);
			var reader = new BinaryReader(fileStream);

			var vox = reader.ReadChars(4).Select(c => c.ToString()).Aggregate((a, c) => a += c);
			var version = reader.ReadInt32();
			
			//Debug.Log(vox+ " version: "+ version );

			models = new List<Model>();
			materials = new List<Vox_Material>();
			
			ReadChunk(reader);
			
			reader.Close();
			reader.Dispose();

			if (palette == null)
			{
				palette = new Color32[256];
				for (int i = 0; i < 256; ++i)
				{
					palette[i].r = Convert.ToByte(defaultPalette[i] & 0x000000ff);
					palette[i].g = Convert.ToByte( ( defaultPalette[i] >> 2 ) & 0x000000ff);
					palette[i].b = Convert.ToByte( ( defaultPalette[i] >> 4 ) & 0x000000ff);
					palette[i].a = Convert.ToByte( ( defaultPalette[i] >> 6 ) & 0x000000ff);
				}
			}

			paletteTexture = new Texture2D( 256, 4, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None );
			paletteTexture.name = "Palette";
			paletteTexture.filterMode = FilterMode.Point;
			paletteTexture.wrapMode = TextureWrapMode.Clamp;
			
			Color[] pixelValues = new Color[1024];
			
			for (int i = 0; i < 256; ++i)
				pixelValues[i] = palette[i];
			
			foreach (var material in materials)
			{
				pixelValues[material.id + 256 - 1] = new Color(
					((int) material.type) / 4f,																			// Type
					(1f-material.plastic)*( (material.type == Vox_MaterialType.Metal)? material.materialValue : 0f ),	// Metallic * 1-Plastic
					(material.type == Vox_MaterialType.Glass)? material.materialValue : 1f,								// Alpha
					(material.type == Vox_MaterialType.Emissive)? material.materialValue : 0f							// Emission 
				);
				
				pixelValues[material.id + 512 - 1] = new Color(
					material.plastic,
					material.smoothness,
					material.specular,
					material.ior
				);
				
				pixelValues[material.id + 768 - 1] = new Color(
					material.attenuation,
					material.power,
					material.glow,
					0f
				);
			}
			
			paletteTexture.SetPixels( pixelValues );
			paletteTexture.Apply();
			
			ctx.AddObjectToAsset("Palette", paletteTexture);

			var m = 0;
			
			foreach (var model in models)
			{
				model.GeneratePointCache();
				
				foreach (var surface in model.linkedPointCache.surfaces)
				{
					ctx.AddObjectToAsset( "model_"+m+"_"+surface.name, surface );
				}
				
				ctx.AddObjectToAsset( "model_"+m, model.linkedPointCache);

				++m;
			}

			foreach (var g in vox_Groups)
			{
				vox_Transforms.Find(t => t.childNodeId == g.id).group = g;
				
				foreach (var cid in g.childNodeIds)
				{
					g.children.Add( vox_Transforms.Find(t => t.id == cid) );
				}
			}

			foreach (var s in vox_Shapes)
				vox_Transforms.Find(t => t.childNodeId == s.id).shape = s;

			foreach (var t in vox_Transforms)
				if (!vox_Groups.Exists(g => g.childNodeIds.Contains(t.id))) topmostTransform = t;

			var go = CreateObject(topmostTransform);

			ctx.AddObjectToAsset( Path.GetFileNameWithoutExtension(ctx.assetPath), go);
			ctx.SetMainObject(go);
		}

		int ReadChunk(BinaryReader reader )
		{   
			var id = reader.ReadChars(4).Select(c => c.ToString()).Aggregate((a, c) => a += c);
			var chunkSize = reader.ReadInt32();
			var chunkChildrensSize = reader.ReadInt32();
			
			// Debug.Log($"Chunk \"{id}\" : size={chunkSize} ; childrensSize={chunkChildrensSize}");
			
			ReadChunkData(reader, id, chunkSize);

			while (chunkChildrensSize > 0)
			{
				chunkChildrensSize -= ReadChunk(reader);
			}
			
			return chunkSize + chunkChildrensSize + 12;
		}

		void ReadChunkData( BinaryReader reader, string id, int size)
		{
			Model model;
			
			switch (id)
			{
				case "MAIN": break; // No data in main
				case "PACK":
					modelCount = reader.ReadInt32();
					break;
				case "SIZE":
					model = new Model();
					var modelSize = Vector3Int.zero;
					modelSize.x = reader.ReadInt32();
					modelSize.y = reader.ReadInt32();
					modelSize.z = reader.ReadInt32();
					models.Add(model);
					break;
				case "XYZI":
					model = models.Last();

					var voxelsCount = reader.ReadInt32();
					
					for (int i = 0; i < voxelsCount; ++i)
					{
						var voxel = new Voxel();
						
						voxel.position.x = Convert.ToInt32( reader.ReadByte() );
						voxel.position.z = Convert.ToInt32( reader.ReadByte() );
						voxel.position.y = Convert.ToInt32( reader.ReadByte() );
						voxel.id = Convert.ToInt32( reader.ReadByte() );
						
						model.voxels.Add(voxel);
					}
					break;
				case "RGBA":
					palette = new Color32[256];
					for (int i = 0; i < 256; ++i)
					{
						palette[i].r = reader.ReadByte();
						palette[i].g = reader.ReadByte();
						palette[i].b = reader.ReadByte();
						palette[i].a = reader.ReadByte();
					}
					break;
				case "MATL":
					materials.Add( new Vox_Material( reader, true ));
					break;
				case "MATT":
					materials.Add( new Vox_Material( reader ) );
					break;
				case "nTRN":
					vox_Transforms.Add( new Vox_Transform( reader ) );
					break;
				case "nGRP":
					vox_Groups.Add( new Vox_Group( reader ) );
					break;
				case "nSHP":
					vox_Shapes.Add(new Vox_Shape( reader ));
					break;
				case "LAYR":
					vox_Layers.Add(new Vox_Layer( reader ));
					break;
				default:
					var trashBytes = new byte[size];
					reader.Read(trashBytes, 0, (int) size);
					break;
			}
		}

		GameObject CreateObject(Vox_Transform vt )
		{
			var go = new GameObject();
			go.name = vt.name;
			go.SetActive( !vt.hidden );

			go.transform.localPosition = vt.position;
			go.transform.localEulerAngles = vt.rotation;

			if ( vt.group?.childCount > 0)
			{
				foreach (var child in vt.group.children)
				{
					CreateObject(child).transform.parent = go.transform;
				}
			}

			if (vt.shape != null)
			{
				BuildShapeToObject(go, vt.shape);			
			}

			return go;
		}

		void BuildShapeToObject(GameObject go, Vox_Shape shape)
		{
			var voxShape = go.AddComponent<VoxShape>();

			var model = models[ shape.modelDatas[0].id ];

			model.linkedPointCache.name = go.name;
			voxShape.pointCacheGUID = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( model.linkedPointCache ) ) ;
			voxShape.palette = paletteTexture;
		}

		[Serializable]
		public class Model
		{
			public Vector3Int size;

			[HideInInspector]
			public List<Voxel> voxels;

			public Model()
			{
				size = Vector3Int.one;
				voxels = new List<Voxel>();
			}

			public Vector2Int attributeMapSize;

			public PointCacheAsset linkedPointCache;

			public void GeneratePointCache()
			{
				var pCache = new PCache();
				pCache.AddVector4Property("position_index");
				pCache.SetVector4Data("position_index", voxels.Select(v => new Vector4(v.position.x, v.position.y, v.position.z, v.id)).ToList());

				linkedPointCache = PointCacheUtil.GeneratePointCacheAsset(pCache);
				
				attributeMapSize = new Vector2Int( linkedPointCache.surfaces[0].width, linkedPointCache.surfaces[0].height );
			}
		}

		[Serializable]
		public struct Voxel
		{
			public Vector3Int position;
			[FormerlySerializedAs("index")]
			public int id;
		}

		public enum Vox_MaterialType {
			Diffuse = 0,
			Metal = 1,
			Glass = 2,
			Emissive = 3
		}
		
		[Serializable]
		public struct Vox_Material
		{
			// id [1-255]
			public int id;
			
			/*
				material type
				0 : diffuse
				1 : metal
				2 : glass
				3 : emissive
			*/
			public Vox_MaterialType type;
			
			/*
				material weight
				diffuse  : 1.0
				metal    : (0.0 - 1.0] : blend between metal and diffuse material
				glass    : (0.0 - 1.0] : blend between glass and diffuse material
				emissive : (0.0 - 1.0] : self-illuminated material
			*/
			public float materialValue;
			
			/*
				property bits : set if value is saved in next section
				bit(0) : Plastic
				bit(1) : Roughness
				bit(2) : Specular
				bit(3) : IOR
				bit(4) : Attenuation
				bit(5) : Power
				bit(6) : Glow
				bit(7) : isTotalPower (*no value)
			*/
			public int propertyBits;

			/*
				normalized property value : (0.0 - 1.0]
					need to map to real range
					Plastic material only accepts {0.0, 1.0} for this version
			*/
			float[] normalizedPropertyValues;

			// Public accessible values.
			public float plastic;
			public float smoothness; // Invert the roughness as Unity works on smoothness base materials
			public float specular;
			public float ior;
			public float attenuation;
			public float power;
			public float glow;

			public Vox_Material( BinaryReader reader, bool isMATL = false )
			{
				type = Vox_MaterialType.Diffuse;
				materialValue = 0f;
				propertyBits = 0;

				if (isMATL)
				{
					id = reader.ReadInt32();
					normalizedPropertyValues = new float[8];

					var data = ReadVoxDictionary(reader);
					float floatValue;
					foreach (var kv in data)
					{
						float.TryParse(kv.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue);
						
						switch (kv.Key)
						{
							case "_type" :
								switch (kv.Value)
								{
									case "_metal": type = Vox_MaterialType.Metal ; break;
									case "_glass": type = Vox_MaterialType.Glass ; break;
									case "_emit": type = Vox_MaterialType.Emissive ; break;
									default: type = Vox_MaterialType.Diffuse; break;
											
								}
								break;
							case "_weight": materialValue = floatValue; break;
							case "_plastic":
								propertyBits |= 1;
								normalizedPropertyValues[0] = floatValue;
								break; 
							case "_rough":
								propertyBits |= 1 << 1;
								normalizedPropertyValues[1] = floatValue;
								break;
							case "_spec" :
								propertyBits |= 1 << 2;
								normalizedPropertyValues[2] = floatValue;
								break;
							case "_ior" :
								propertyBits |= 1 << 3;
								normalizedPropertyValues[3] = floatValue;
								break;
							case "_att" :
								propertyBits |= 1 << 4;
								normalizedPropertyValues[4] = floatValue;
								break;
							case "_flux" :
								propertyBits |= 1 << 5;
								normalizedPropertyValues[5] = floatValue;
								break;
						}
					}
				}
				else
				{
					id = reader.ReadInt32();
					type = (Vox_MaterialType) reader.ReadInt32();
					materialValue = reader.ReadSingle();
					propertyBits = reader.ReadInt32();
					normalizedPropertyValues = new float[8];
					for (var b = 0; b < 7; ++b)
					{
						if ( ((propertyBits >> b) & 1) != 0 )
						{
							normalizedPropertyValues[b] = reader.ReadSingle();
						}
					}
				}

				plastic = 		normalizedPropertyValues[0];
				smoothness = 1f-normalizedPropertyValues[1];
				specular = 		normalizedPropertyValues[2];
				ior = 			normalizedPropertyValues[3];
				attenuation = 	normalizedPropertyValues[4];
				power = 		normalizedPropertyValues[5];
				glow = 			normalizedPropertyValues[6];
			}
		}
		
		[Serializable]
		public class Vox_Transform
		{
			public int id;
			public Dictionary<string, string> attributes;
			public int childNodeId;
			public int reservedID;
			public int layerID;
			public int numOfFrames;
			public List<Vox_FrameAttribute> frameAttributes;

			public string name;
			public bool hidden;
			
			public Vox_Transform(BinaryReader reader)
			{
				id = reader.ReadInt32();
				attributes = ReadVoxDictionary(reader);
				childNodeId = reader.ReadInt32();
				reservedID = reader.ReadInt32();
				layerID = reader.ReadInt32();
				numOfFrames = reader.ReadInt32();
				frameAttributes = new List<Vox_FrameAttribute>(numOfFrames);
				for (var i = 0; i < numOfFrames; ++i)
				{
					frameAttributes.Add( new Vox_FrameAttribute(reader) );
				}

				name = "Object";
				hidden = false;
				if (attributes.ContainsKey("_name")) name = attributes["_name"];
				if (attributes.ContainsKey("_hidden "))
				{
					int h = 0;
					int.TryParse(attributes["_hidden"], NumberStyles.Integer, CultureInfo.InvariantCulture, out h);
					hidden = h == 1;
				}
				
				position = Vector3.zero;
				rotation = Vector3.zero;

				if (frameAttributes.Count > 0)
				{
					position = frameAttributes[0].position;
					rotation = frameAttributes[0].rotation;
				}
			}

			public Vector3 position;
			public Vector3 rotation;
			
			public Vox_Group group;
			public Vox_Shape shape;
		}

		[Serializable]
		public class Vox_FrameAttribute
		{
			public Dictionary<string, string> data;
			public Vector3 position;
			public Vector3 rotation;

			public Vox_FrameAttribute(BinaryReader reader)
			{
				data = ReadVoxDictionary(reader);
				position = Vector3.zero;
				rotation = Vector3.zero;
				
				if (data.ContainsKey("_t"))
				{
					var posData = data["_t"].Split(" "[0]);
					float.TryParse(posData[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out position.x);
					float.TryParse(posData[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out position.z);
					float.TryParse(posData[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out position.y);
				}

				if (data.ContainsKey("_r"))
				{
					var rotData = 0 << 0 | 1 << 2 | 0 << 4 | 0 << 5 | 0 << 6;
					int.TryParse(data["_r"], NumberStyles.Integer, CultureInfo.InvariantCulture, out rotData);
					
					rotation = RotationMatrixToEulerAngles( DecodeVoxRotationMatrix( (byte) rotData ) );
				}
			}
		}

		[Serializable]
		public class Vox_Group
		{
			public int id;
			public Dictionary<string, string> attributes;
			public int childCount;
			public int[] childNodeIds;

			public Vox_Group(BinaryReader reader)
			{   
				id = reader.ReadInt32();
				attributes = ReadVoxDictionary(reader);
				childCount = reader.ReadInt32();
				childNodeIds = new int[childCount];
				for (int i = 0; i < childCount; ++i)
				{
					childNodeIds[i] = reader.ReadInt32();
				}
				
				children = new List<Vox_Transform>();
			}

			public List<Vox_Transform> children;
		}
		
		[Serializable]
		public class Vox_Shape
		{
			public int id;
			public Dictionary<string, string> attributes;
			public int modelCount;
			public List<Vox_ShapeModelData> modelDatas;

			public Vox_Shape( BinaryReader reader )
			{
				id = reader.ReadInt32();
				attributes = ReadVoxDictionary(reader);
				modelCount = reader.ReadInt32();
				modelDatas = new List<Vox_ShapeModelData>();
				for (int i = 0; i < modelCount; i++)
				{
					modelDatas.Add(new Vox_ShapeModelData(reader));
				}
			}
		}

		[Serializable]
		public class Vox_ShapeModelData
		{
			public int id;
			public Dictionary<string, string> attributes;

			public Vox_ShapeModelData(BinaryReader reader)
			{
				id = reader.ReadInt32();
				attributes = ReadVoxDictionary(reader);
			}
		}
		
		[Serializable]
		public class Vox_Layer
		{
			public int id;
			public Dictionary<string, string> attributes;
			public int reservedId;

			public Vox_Layer(BinaryReader reader)
			{
				id = reader.ReadInt32();
				attributes = ReadVoxDictionary(reader);
				reservedId = reader.ReadInt32();
			}
		}

		public static Dictionary<string, string> ReadVoxDictionary(BinaryReader reader)
		{
			var outDictionary = new Dictionary<string, string>();
			
			var entries = reader.ReadInt32();
			string key;
			for (var i = 0; i < entries; ++i)
			{
				key = ReadVoxString(reader);
				
				outDictionary.Add(key, ReadVoxString(reader) );
			}

			return outDictionary;
		}

		public static string ReadVoxString(BinaryReader reader)
		{
			var length = reader.ReadInt32();
			return new string(reader.ReadChars(length));
		}

		public static Vector3 ReadVoxRotation(BinaryReader reader)
		{
			var value = reader.ReadByte();

			return RotationMatrixToEulerAngles(DecodeVoxRotationMatrix(value));
		}

		public static Matrix4x4 DecodeVoxRotationMatrix(byte value)
		{
			var index0 = value & 3;
			var index1 = (value >> 2) & 3;
			var index2 = 3 - index0 - index1;

			var matrix = Matrix4x4.zero;
			matrix[0, index0] = ( ( (value >> 4) & 1 ) == 1)?1: -1;
			matrix[1, index1] = ( ( (value >> 5) & 1 ) == 1)?1: -1;
			matrix[3, index2] = ( ( (value >> 6) & 1 ) == 1)?1: -1;
			matrix[3, 3] = 1;

			return matrix;
		}
		
		public static Vector3 RotationMatrixToEulerAngles(Matrix4x4 matrix)
		{
			float sy = Mathf.Sqrt(matrix[0,0] * matrix[0,0] +  matrix[1,0] * matrix[1,0] );
	
			bool singular = sy < 1e-6; // If
	
			float x, y, z;
			if (!singular)
			{
				x = Mathf.Atan2(matrix[2,1] , matrix[2,2] );
				y = Mathf.Atan2(-matrix[2,0], sy);
				z = Mathf.Atan2(matrix[1,0], matrix[0,0]);
			}
			else
			{
				x = Mathf.Atan2(-matrix[1,2], matrix[1,1]);
				y = Mathf.Atan2(-matrix[2,0], sy);
				z = 0;
			}
			return new Vector3(x, y, z);
		}
	}

}