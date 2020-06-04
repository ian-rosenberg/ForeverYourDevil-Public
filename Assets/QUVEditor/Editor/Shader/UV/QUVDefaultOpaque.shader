Shader "Hidden/QTools/QUVDefaultOpaque" 
{
	Properties 
	{
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200		
		Lighting off
		Cull off
		
		PASS 
		{
			CGPROGRAM
			#pragma vertex vert
        	#pragma fragment frag
			
			struct appdata 
	        {	
	            float4 vertex: POSITION;	
	            float4 color : COLOR;		
	        };
			
			struct v2f 
			{				
	            float4 pos   : SV_POSITION;	
	            float4 color : COLOR;		
	        };		 	         
		        	
	        v2f vert(appdata v)	
	        {	
	            v2f o;	
	            o.pos = UnityObjectToClipPos(v.vertex);	
	            o.color = v.color; 
	            return o;	
	        }	

	        float4 frag(v2f IN): COLOR	
	        {		        	        	
	            return IN.color;
	        }
			
			ENDCG
		}
	}
}
