using UnityEngine;
using System.Collections;

public class TestView : ViewBase {
    public UILabel lbl_lbl_name; 
    public UILabel lbl_lbl_name1; 
    public UILabel lbl_lbl_title; 
    public UISprite sp_sp_avatar; 
    public UISprite sp_btn_back; 
    public UISprite sp_btn_back1; 
    public UITexture tex_tex_bg; 
    public UIButton btn_btn_back; 
    public UIButton btn_btn_back1; 


	public override void Start () {
        UIEventListener.Get(btn_btn_back.gameObject).onClick = btn_back_onclick;
        UIEventListener.Get(btn_btn_back1.gameObject).onClick = btn_back1_onclick;

	}

	public override void Update () {
		
	}

	public override void OnEnable () {
		
	}

	public override void OnDisable () {
		
	}

    void btn_back_onclick (GameObject go) { 

	} 

    void btn_back1_onclick (GameObject go) { 

	} 


}
