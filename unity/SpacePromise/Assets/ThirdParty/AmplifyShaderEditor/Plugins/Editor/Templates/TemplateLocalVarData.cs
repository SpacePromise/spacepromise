using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateLocalVarData
	{
		[SerializeField]
		private WirePortDataType m_dataType = WirePortDataType.OBJECT;

		[SerializeField]
		private string m_localVarName = string.Empty;

		[SerializeField]
		private int m_position = -1;
		
		[SerializeField]
		private TemplateInputData m_inputData = null;
		
		public TemplateLocalVarData( WirePortDataType dataType, string localVarName, int position, TemplateInputData inputData )
		{
			m_dataType = dataType;
			m_localVarName = localVarName;
			m_position = position;
			m_inputData = new TemplateInputData( inputData );
			//Debug.Log( m_localVarName + " " + m_inputData.PortCategory + " " + m_inputData.PortName );
		}

		public WirePortDataType DataType { get { return m_dataType; } }
		public string LocalVarName { get { return m_localVarName; } }
		public int Position { get { return m_position; } }
		public TemplateInputData InputData { get { return m_inputData; } }
	}
}
