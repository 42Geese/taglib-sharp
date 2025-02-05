//
// AudioFile.cs: Provides tagging and properties support for MPEG-1, MPEG-2, and
// MPEG-2.5 audio files.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   mpegfile.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2002, 2003 by Scott Wheeler (Original Implementation)
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;

namespace TagLib.Mpeg
{
	/// <summary>
	///    This class extends <see cref="TagLib.NonContainer.File" /> to
	///    provide tagging and properties support for MPEG-1, MPEG-2, and
	///    MPEG-2.5 audio files.
	/// </summary>
	/// <remarks>
	///    A <see cref="TagLib.Id3v1.Tag" /> and <see
	///    cref="TagLib.Id3v2.Tag" /> will be added automatically to any
	///    file that doesn't contain one. This change does not effect the
	///    file until it is saved and can be reversed using the following
	///    method:
	///    <code>file.RemoveTags (file.TagTypes &amp; ~file.TagTypesOnDisk);</code>
	/// </remarks>
	[SupportedMimeType ("taglib/mp3", "mp3")]
	[SupportedMimeType ("audio/x-mp3")]
	[SupportedMimeType ("application/x-id3")]
	[SupportedMimeType ("audio/mpeg")]
	[SupportedMimeType ("audio/x-mpeg")]
	[SupportedMimeType ("audio/x-mpeg-3")]
	[SupportedMimeType ("audio/mpeg3")]
	[SupportedMimeType ("audio/mp3")]
	[SupportedMimeType ("taglib/m2a", "m2a")]
	[SupportedMimeType ("taglib/mp2", "mp2")]
	[SupportedMimeType ("taglib/mp1", "mp1")]
	[SupportedMimeType ("audio/x-mp2")]
	[SupportedMimeType ("audio/x-mp1")]
	public class AudioFile : TagLib.NonContainer.File
	{
		#region Private Fields

		/// <summary>
		///    Contains the first audio header.
		/// </summary>
		AudioHeader first_header;

		#endregion


		#region Private Static Fields

		/// <summary>
		///    Specifies whether or not to create ID3v1 
		///    tag when it doesn't exist.
		/// </summary>
		private static bool create_id3_v1_tags = true;

		/// <summary>
		///    Specifies whether or not to create ID3v2 
		///    tag when it doesn't exist.
		/// </summary>
		private static bool create_id3_v2_tags = true;

		#endregion


		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AudioFile" /> for a specified path in the local
		///    file system and specified read style.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public AudioFile (string path, ReadStyle propertiesStyle)
			: base (path, propertiesStyle)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AudioFile" /> for a specified path in the local
		///    file system with an average read style.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public AudioFile (string path)
			: base (path)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AudioFile" /> for a specified file abstraction and
		///    specified read style.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="TagLib.File.IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public AudioFile (IFileAbstraction abstraction, ReadStyle propertiesStyle)
			: base (abstraction, propertiesStyle)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AudioFile" /> for a specified file abstraction with
		///    an average read style.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="TagLib.File.IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public AudioFile (IFileAbstraction abstraction)
			: base (abstraction)
		{
		}

		#endregion


		#region Public Static Properties

		/// <summary>
		///    Gets and sets whether or not to create ID3v1 and
		///    ID3v2 tags automatically when they are not existing.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if tags to be created automatically.
		///    Otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		///    <para>Sometimes a MP3 file should only contain ID3v1 and no
		///    ID3v2 Tags. Or instead of ID3v2 Tags APE Tags should be used.
		///    By setting this property to <see langword="false" />,
		///    no ID3v1 and Id3v2 Tags will be created when creating the file,
		///    if they don't exist.
		///    They need to be created explicitly if needed.</para>
		///    <para>The default is <see langword="true" /> which means that
		///    ID3v1 and Id3v2 tags are created when they don't exist.</para>
		/// </remarks>
		public static bool CreateID3Tags {
			get { 
				if (create_id3_v1_tags && create_id3_v2_tags) {
					return true;
				} else if (!create_id3_v1_tags && !create_id3_v2_tags) {
					return false;
				}

				throw new InvalidOperationException("Inconsistent V1 and V2 state.");
			}
			set { 
				create_id3_v1_tags = value; 
				create_id3_v2_tags = value;
			}
		}

		/// <summary>
		///    Gets and sets whether or not to create an ID3v1 tag 
		///    automatically when it is not existing.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the tag is to be created automatically.
		///    Otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		///    <para>Sometimes an MP3 file should not contain an ID3v1 Tag.
		///    By setting this property to <see langword="false" />,
		///    no ID3v1 Tag will be created when creating the file,
		///    if it doesn't exist.
		///    It needs to be created explicitly if needed.</para>
		///    <para>The default is <see langword="true" /> which means that
		///    an ID3v1 tag is created when it doesn't exist.</para>
		/// </remarks>
		public static bool CreateID3V1Tags {
			get { return create_id3_v1_tags; }
			set { create_id3_v1_tags = value; }
		}

		/// <summary>
		///    Gets and sets whether or not to create an ID3v2 tag 
		///    automatically when it is not existing.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the tag is to be created automatically.
		///    Otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		///    <para>Sometimes an MP3 file should not contain an ID3v2 Tag.
		///    By setting this property to <see langword="false" />,
		///    no ID3v1 Tag will be created when creating the file,
		///    if it doesn't exist.
		///    It needs to be created explicitly if needed.</para>
		///    <para>The default is <see langword="true" /> which means that
		///    an ID3v2 tag is created when it doesn't exist.</para>
		/// </remarks>
		public static bool CreateID3V2Tags {
			get { return create_id3_v2_tags; }
			set { create_id3_v2_tags = value; }
		}

		#endregion


		#region Public Methods

		/// <summary>
		///    Gets a tag of a specified type from the current instance,
		///    optionally creating a new tag if possible.
		/// </summary>
		/// <param name="type">
		///    A <see cref="TagLib.TagTypes" /> value indicating the
		///    type of tag to read.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> value specifying whether or not to
		///    try and create the tag if one is not found.
		/// </param>
		/// <returns>
		///    A <see cref="Tag" /> object containing the tag that was
		///    found in or added to the current instance. If no
		///    matching tag was found and none was created, <see
		///    langword="null" /> is returned.
		/// </returns>
		/// <remarks>
		///    If a <see cref="TagLib.Id3v2.Tag" /> is added to the
		///    current instance, it will be placed at the start of the
		///    file. On the other hand, <see cref="TagLib.Id3v1.Tag" />
		///    <see cref="TagLib.Ape.Tag" /> will be added to the end of
		///    the file. All other tag types will be ignored.
		/// </remarks>
		public override Tag GetTag (TagTypes type, bool create)
		{
			Tag t = (Tag as TagLib.NonContainer.Tag).GetTag (type);

			if (t != null || !create)
				return t;

			switch (type) {
			case TagTypes.Id3v1:
				return EndTag.AddTag (type, Tag);

			case TagTypes.Id3v2:
				return StartTag.AddTag (type, Tag);

			case TagTypes.Ape:
				return EndTag.AddTag (type, Tag);

			default:
				return null;
			}
		}

		#endregion



		#region Protected Methods

		/// <summary>
		///    Reads format specific information at the start of the
		///    file.
		/// </summary>
		/// <param name="start">
		///    A <see cref="long" /> value containing the seek position
		///    at which the tags end and the media data begins.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <remarks>
		///    This method only searches for an audio header in the
		///    first 16384 bytes of code to avoid searching forever in
		///    corrupt files.
		/// </remarks>
		protected override void ReadStart (long start, ReadStyle propertiesStyle)
		{
			// Only check the first 16 bytes so we're not stuck
			// reading a bad file forever.
			if ((propertiesStyle & ReadStyle.Average) != 0 &&
				!AudioHeader.Find (out first_header, this, start, 0x4000))
				throw new CorruptFileException ("MPEG audio header not found.");
		}

		/// <summary>
		///    Reads format specific information at the end of the
		///    file.
		/// </summary>
		/// <param name="end">
		///    A <see cref="long" /> value containing the seek position
		///    at which the media data ends and the tags begin.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		protected override void ReadEnd (long end, ReadStyle propertiesStyle)
		{
			// Creation of ID3v1 and ID3v2 tags based on CreateID3Tags property
			GetTag (TagTypes.Id3v1, create_id3_v1_tags);
			GetTag (TagTypes.Id3v2, create_id3_v2_tags);
		}

		/// <summary>
		///    Reads the audio properties from the file represented by
		///    the current instance.
		/// </summary>
		/// <param name="start">
		///    A <see cref="long" /> value containing the seek position
		///    at which the tags end and the media data begins.
		/// </param>
		/// <param name="end">
		///    A <see cref="long" /> value containing the seek position
		///    at which the media data ends and the tags begin.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <returns>
		///    A <see cref="TagLib.Properties" /> object describing the
		///    media properties of the file represented by the current
		///    instance.
		/// </returns>
		protected override Properties ReadProperties (long start, long end, ReadStyle propertiesStyle)
		{
			first_header.SetStreamLength (end - start);
			return new Properties (TimeSpan.Zero, first_header);
		}

		#endregion
	}
}
