// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Data.WindowsInstaller
{
    using System.Xml;

    /// <summary>
    /// Substorage inside an output.
    /// </summary>
    public sealed class SubStorage
    {
        /// <summary>
        /// Instantiate a new substorage.
        /// </summary>
        /// <param name="name">The substorage name.</param>
        /// <param name="data">The substorage data.</param>
        public SubStorage(string name, WindowsInstallerData data)
        {
            this.Name = name;
            this.Data = data;
        }

        /// <summary>
        /// Gets the substorage name.
        /// </summary>
        /// <value>The substorage name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the substorage data.
        /// </summary>
        /// <value>The substorage data.</value>
        public WindowsInstallerData Data { get; }

        /// <summary>
        /// Creates a SubStorage from the XmlReader.
        /// </summary>
        /// <param name="reader">Reader to get data from.</param>
        /// <param name="tableDefinitions">Table definitions to use for strongly-typed rows.</param>
        /// <returns>New SubStorage object.</returns>
        internal static SubStorage Read(XmlReader reader, TableDefinitionCollection tableDefinitions)
        {
            if (reader.LocalName != "subStorage")
            {
                throw new XmlException();
            }

            WindowsInstallerData data = null;
            string name = null;

            var empty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.Value;
                        break;
                }
            }

            if (!empty)
            {
                var done = false;

                while (!done && reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                case WindowsInstallerData.XmlElementName:
                                    data = WindowsInstallerData.Read(reader, tableDefinitions, true);
                                    break;
                                default:
                                    throw new XmlException();
                            }
                            break;

                        case XmlNodeType.EndElement:
                            done = true;
                            break;
                    }
                }

                if (!done)
                {
                    throw new XmlException();
                }
            }

            return new SubStorage(name, data);
        }

        /// <summary>
        /// Persists a SubStorage in an XML format.
        /// </summary>
        /// <param name="writer">XmlWriter where the SubStorage should persist itself as XML.</param>
        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("subStorage", WindowsInstallerData.XmlNamespaceUri);

            writer.WriteAttributeString("name", this.Name);

            this.Data.Write(writer);

            writer.WriteEndElement();
        }
    }
}
