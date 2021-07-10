using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SoundManager : Singleton<SoundManager>
{
	private GameObject _SFXPrefab;
	private Dictionary<string, List<AudioClip>> _Sounds = new Dictionary<string, List<AudioClip>>();
	private List<AudioSource> _Channels = new List<AudioSource>();
	private int _PlayingChannelsCount;
	StringBuilder _StringBuilder = new StringBuilder();


	private void Awake()
	{
		_SFXPrefab = new GameObject("SFX", typeof(AudioSource));
		_PlayingChannelsCount = 0;

		LoadDirectories("Sounds/");
	}

	private void LoadDirectories(string path)
	{
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/" + path);
		foreach (var iter in dir.GetDirectories())
		{
			LoadDirectories(path + iter.Name + "/");
		}
		if (dir.Name.StartsWith("Set_"))
		{
			_StringBuilder.Clear();
			_StringBuilder.Append(path);
			List<AudioClip> temp = new List<AudioClip>();
			_Sounds.Add(dir.Name.Remove(0,4), temp);
			foreach (var file in dir.GetFiles())
			{
				if (file.Extension.Equals(".meta"))
					continue;
				_StringBuilder.Append(file.Name);
				_StringBuilder.Remove(_StringBuilder.Length - 4, 4);
				temp.Add(Resources.Load<AudioClip>(_StringBuilder.ToString()));
				_StringBuilder.Remove(path.Length, _StringBuilder.Length - path.Length);
			}
		}
		else
		{
			_StringBuilder.Clear();
			_StringBuilder.Append(path);
			foreach (var file in dir.GetFiles())
			{
				if (file.Extension.Equals(".meta"))
					continue;
				List<AudioClip> temp = new List<AudioClip>();
				_StringBuilder.Append(file.Name);
				_StringBuilder.Remove(_StringBuilder.Length - 4, 4);
				temp.Add(Resources.Load<AudioClip>(_StringBuilder.ToString()));
				_StringBuilder.Remove(path.Length, _StringBuilder.Length - path.Length);
				_Sounds.Add(file.Name.Remove(file.Name.Length - 4, 4), temp);
			}
		}
	}

	private void Update()
	{
		AudioSource temp;
		for (int i = 0; i < _PlayingChannelsCount; i++)
		{
			if (_Channels[i].isPlaying == false)
			{
				_Channels[i].gameObject.SetActive(false);
				_PlayingChannelsCount--;
				temp = _Channels[_PlayingChannelsCount];
				_Channels[_PlayingChannelsCount] = _Channels[i];
				_Channels[i] = temp;
			}
		}
	}

	public AudioSource Play(string key, float volume = 1, bool isLoop = false)
	{
		AudioSource temp;
		if(_PlayingChannelsCount == _Channels.Count)
		{
			temp = Instantiate(_SFXPrefab, transform).GetComponent<AudioSource>();
			_Channels.Add(temp);
		}
		else
		{
			temp = _Channels[_PlayingChannelsCount];
			temp.gameObject.SetActive(true);
		}
		List<AudioClip> setTemp = _Sounds[key];
		temp.clip = setTemp[Random.Range(0, setTemp.Count)];
		temp.Play();
		temp.volume = volume;
		temp.loop = isLoop;
		_PlayingChannelsCount++;
		return temp;
	}
}
