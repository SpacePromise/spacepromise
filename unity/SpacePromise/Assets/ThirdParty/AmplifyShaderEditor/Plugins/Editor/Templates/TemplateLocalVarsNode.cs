// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Template Local Var Data", "Surface Data", "Select and use available local variable data from the template" )]
	public class TemplateLocalVarsNode : ParentNode
	{
		private const string ErrorMessageStr = "This node can only be used inside a Template category!";
		private const string DataLabelStr = "Data";

		private List<TemplateLocalVarData> m_localVarsData = null;

		[SerializeField]
		private int m_currentDataIdx = -1;

		[SerializeField]
		private string m_dataName = string.Empty;

		private string[] m_dataLabels = null;

		private bool m_fetchDataId = false;
		private UpperLeftWidgetHelper m_upperLeftWidgetHelper = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			AddOutputPort( WirePortDataType.FLOAT, "X" );
			AddOutputPort( WirePortDataType.FLOAT, "Y" );
			AddOutputPort( WirePortDataType.FLOAT, "Z" );
			AddOutputPort( WirePortDataType.FLOAT, "W" );
			m_textLabelWidth = 67;
			m_hasLeftDropdown = true;
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		void ConfigurePorts()
		{
			switch( m_outputPorts[ 0 ].DataType )
			{
				default:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = false;
					}
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = ( i < 3 );
						if( m_outputPorts[ i ].Visible )
						{
							m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
						}
					}
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = ( i < 4 );
						if( m_outputPorts[ i ].Visible )
						{
							m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
						}
					}
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = true;
						m_outputPorts[ i ].Name = Constants.ChannelNamesVector[ i - 1 ];
					}
				}
				break;
				case WirePortDataType.COLOR:
				{
					for( int i = 1; i < 5; i++ )
					{
						m_outputPorts[ i ].Visible = true;
						m_outputPorts[ i ].Name = Constants.ChannelNamesColor[ i - 1 ];
					}
				}
				break;
			}
			m_sizeIsDirty = true;
		}

		void FetchDataId()
		{
			if( m_localVarsData != null )
			{
				m_currentDataIdx = 0;
				int count = m_localVarsData.Count;
				m_dataLabels = new string[ count ];
				for( int i = 0; i < count; i++ )
				{
					m_dataLabels[ i ] = m_localVarsData[ i ].LocalVarName;
					if( m_localVarsData[ i ].LocalVarName.Equals( m_dataName ) )
					{
						m_currentDataIdx = i;
					}
				}
				UpdateFromId();
			}
			else
			{
				m_currentDataIdx = -1;
			}
		}

		void UpdateFromId()
		{
			if( m_localVarsData != null )
			{
				bool areCompatible = TemplateHelperFunctions.CheckIfCompatibles( m_outputPorts[ 0 ].DataType, m_localVarsData[ m_currentDataIdx ].DataType );
				string category = m_localVarsData[ m_currentDataIdx ].InputData.PortCategory == MasterNodePortCategory.Fragment ? "Surface Data" : "Vertex Data";
				m_headerColor = UIUtils.GetColorFromCategory( category );
				switch( m_localVarsData[ m_currentDataIdx ].DataType )
				{
					default:
					case WirePortDataType.INT:
					case WirePortDataType.FLOAT:
					m_outputPorts[ 0 ].ChangeProperties( Constants.EmptyPortValue, m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT2:
					m_outputPorts[ 0 ].ChangeProperties( "XY", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT3:
					m_outputPorts[ 0 ].ChangeProperties( "XYZ", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.FLOAT4:
					m_outputPorts[ 0 ].ChangeProperties( "XYZW", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
					case WirePortDataType.COLOR:
					m_outputPorts[ 0 ].ChangeProperties( "RGBA", m_localVarsData[ m_currentDataIdx ].DataType, false );
					break;
				}

				ConfigurePorts();

				if( !areCompatible )
				{
					m_containerGraph.DeleteConnection( false, UniqueId, 0, false, true );
				}

				m_dataName = m_localVarsData[ m_currentDataIdx ].LocalVarName;
				m_content.text = m_dataName;
				m_sizeIsDirty = true;
				CheckWarningState();
			}
		}

		void CheckWarningState()
		{
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
			{
				ShowTab( NodeMessageType.Error, ErrorMessageStr );
			}
			else
			{
				m_showErrorMessage = false;
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			if( m_currentDataIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentDataIdx = EditorGUILayoutPopup( DataLabelStr, m_currentDataIdx, m_dataLabels );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( m_containerGraph.CurrentCanvasMode != NodeAvailability.TemplateShader )
				return;

			if( m_localVarsData == null || m_localVarsData.Count == 0 )
			{
				MasterNode masterNode = m_containerGraph.CurrentMasterNode;
				if( masterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
				{
					TemplateData currentTemplate = ( masterNode as TemplateMasterNode ).CurrentTemplate;
					if( currentTemplate != null )
					{
						m_localVarsData = currentTemplate.LocalVarsList;
						m_fetchDataId = true;
					}
				}
			}

			if( m_fetchDataId )
			{
				m_fetchDataId = false;
				FetchDataId();
			}

			if( m_currentDataIdx > -1 )
			{
				EditorGUI.BeginChangeCheck();
				m_currentDataIdx = m_upperLeftWidgetHelper.DrawWidget( this, m_currentDataIdx, m_dataLabels );
				if( EditorGUI.EndChangeCheck() )
				{
					UpdateFromId();
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_localVarsData[ m_currentDataIdx ].InputData.PortCategory != dataCollector.PortCategory )
			{
				UIUtils.ShowMessage( string.Format( "Local Var {0} can only work on ports of type {1}", m_localVarsData[ m_currentDataIdx ].LocalVarName, m_localVarsData[ m_currentDataIdx ].InputData.PortCategory ) );
				return m_outputPorts[ 0 ].ErrorValue;
			}
			
			return GetOutputVectorItem( 0, outputId, m_dataName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_dataName = GetCurrentParam( ref nodeParams );
			m_fetchDataId = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_dataName );
		}

		public override void OnMasterNodeReplaced( MasterNode newMasterNode )
		{
			base.OnMasterNodeReplaced( newMasterNode );
			if( newMasterNode.CurrentMasterNodeCategory == AvailableShaderTypes.Template )
			{
				TemplateData currentTemplate = ( newMasterNode as TemplateMasterNode ).CurrentTemplate;
				if( currentTemplate != null )
				{
					m_localVarsData = currentTemplate.LocalVarsList;
					FetchDataId();
				}
				else
				{
					m_localVarsData = null;
					m_currentDataIdx = -1;
				}
			}
			else
			{
				m_localVarsData = null;
				m_currentDataIdx = -1;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_dataLabels = null;
			m_localVarsData = null;
			m_upperLeftWidgetHelper = null;
		}
	}
}
