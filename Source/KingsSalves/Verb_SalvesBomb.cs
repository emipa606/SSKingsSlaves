using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace KingsSalves
{
	// Token: 0x02000002 RID: 2
	public class Verb_SalvesBomb : Verb_MeleeAttack
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
		{
			float damAmount = this.verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
			float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
			DamageDef damDef = this.verbProps.meleeDamageDef;
			BodyPartGroupDef bodyPartGroupDef = null;
			HediffDef hediffDef = null;
			damAmount = Rand.Range(damAmount * 0.8f, damAmount * 1.2f);
			bool casterIsPawn = base.CasterIsPawn;
			if (casterIsPawn)
			{
				bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
				bool flag = damAmount >= 1f;
				if (flag)
				{
					bool flag2 = base.HediffCompSource != null;
					if (flag2)
					{
						hediffDef = base.HediffCompSource.Def;
					}
				}
				else
				{
					damAmount = 1f;
					damDef = DamageDefOf.Blunt;
				}
			}
			bool flag3 = base.EquipmentSource != null;
			ThingDef source;
			if (flag3)
			{
				source = base.EquipmentSource.def;
			}
			else
			{
				source = base.CasterPawn.def;
			}
			Vector3 direction = (target.Thing.Position - base.CasterPawn.Position).ToVector3();
			DamageDef def = damDef;
			float num = damAmount;
			float num2 = armorPenetration;
			Thing caster = this.caster;
			DamageInfo mainDinfo = new DamageInfo(def, num, num2, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
			mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
			mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
			mainDinfo.SetWeaponHediff(hediffDef);
			mainDinfo.SetAngle(direction);
			yield return mainDinfo;
			bool flag4 = this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>()) || (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>()));
			if (flag4)
			{
				IEnumerable<ExtraDamage> extraDamages = Enumerable.Empty<ExtraDamage>();
				bool flag5 = this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null;
				if (flag5)
				{
					extraDamages = extraDamages.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
				}
				bool flag6 = this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>();
				if (flag6)
				{
					extraDamages = extraDamages.Concat(this.tool.surpriseAttack.extraMeleeDamages);
				}
				foreach (ExtraDamage extraDamage in extraDamages)
				{
					int extraDamageAmount = GenMath.RoundRandom(extraDamage.AdjustedDamageAmount(this, base.CasterPawn));
					float extraDamageArmorPenetration = extraDamage.AdjustedArmorPenetration(this, base.CasterPawn);
					def = extraDamage.def;
					num2 = (float)extraDamageAmount;
					num = extraDamageArmorPenetration;
					caster = this.caster;
					DamageInfo extraDinfo = new DamageInfo(def, num2, num, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
					extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
					extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
					extraDinfo.SetWeaponHediff(hediffDef);
					extraDinfo.SetAngle(direction);
					yield return extraDinfo;
					extraDinfo = default(DamageInfo);
				}
				IEnumerator<ExtraDamage> enumerator = null;
				extraDamages = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
		{
			DamageWorker.DamageResult result = new DamageWorker.DamageResult();
			foreach (DamageInfo damageInfo in this.DamageInfosToApply(target))
			{
				bool thingDestroyed = target.ThingDestroyed;
				if (thingDestroyed)
				{
					break;
				}
				GenExplosion.DoExplosion(this.caster.Position, this.caster.Map, 2.4f, DamageDefOf.Bomb, this.caster, 200, 3f, null, base.EquipmentSource.def, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
			}
			return result;
		}
	}
}
