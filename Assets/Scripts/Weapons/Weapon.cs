using UnityEngine;

public class Weapon
{
    private string _weaponName;
    private Sprite _sprite;
    private WeaponType _weaponType;
    private int _damage;
    private int _critical;
    private int _range;

    public Weapon(string weaponName, WeaponType weaponType, int damage, int critical, int range)
    {
        _weaponName = weaponName;
        _weaponType = weaponType;
        _damage = damage;
        _critical = critical;
        _range = range;
    }
    
    public string GetWeaponName()
    {
        return _weaponName;
    }
    
    public int GetDamage()
    {
        return _damage;
    }

    public int GetRange()
    {
        return _range;
    }
}