using UnityEngine;
using System.Collections;

/***********************************************************************************************************************************/
/**                                                   ATENCAO                                                                     **/
/** TODAS AS SPELLS DEVERAO SER CARREGADAS ATRAVES DE SEUS IDS.                                                                   **/
/** TODAS AS SPELLS DEVERAO TER SEUS PREFABS SALVOS NA PASTA: ASSETS/RESOURCES/PREFAB/SPELL                                       **/
/** TODAS AS SPELLS DEVERAO SER OBTIDAS ATRAVES DO POOL MANAGER DO APPLICATIONMODEL												  **/
/***********************************************************************************************************************************/

/// <summary>
/// Define uma classe base de habilidade
/// </summary>
public class SpellBase : MonoBehaviour {

	public int ID; // ID da Spell para carregar da Tabela de Habilidaes
	public GameObject SpellObject; /// Responsavel por manter o objeto que representa a spell a ser utilizada seja Mesh, particula, etc...
	public Character Caster; /// Personagem que gerou a habilidade
	public Character Target; // Alvo da Habilidade se houver;
	public AttributeModifier[] AttributeModifiers; /// Modificadores que serao aplicados ao personagem, se houverem;

	// Use this for initialization
	protected virtual void Start () {
		// Inicializa o Array de Modificadores
		AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT];
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}
}


