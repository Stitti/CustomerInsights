import {Button, Card, Code, DataList, Flex, IconButton, Link, Select, Text, TextField} from "@radix-ui/themes";
import {ImageUpload} from "./ImageUpload";
import {useState} from "react";
import {useTranslation} from "react-i18next";
import {CopyIcon, MailIcon, PhoneIcon} from "lucide-react";
import {LanguageSelector} from "./LanguageSelector";

export default function PersonalInformation() {
    const {t} = useTranslation();

    return (
        <Card variant="surface" size="3" mb="6">
            <Flex direction="row" gap="4" p="4" width="60vw">
                <ImageUpload/>
                <Flex direction="column" gap="4" p="4" width="60vw">
                    <DataList.Root size="3">
                        <DataList.Item align="center">
                            <DataList.Label>Firstname</DataList.Label>
                            <DataList.Value style={{ flex: 1 }}>
                                <TextField.Root
                                    style={{ width: "100%" }}
                                    type="text"
                                    value="Jonas"
                                />
                            </DataList.Value>
                        </DataList.Item>

                        <DataList.Item align="center">
                            <DataList.Label>Lastname</DataList.Label>
                            <DataList.Value style={{ flex: 1 }}>
                                <TextField.Root
                                    style={{ width: "100%" }}
                                    type="text"
                                    value="Schneider"
                                />
                            </DataList.Value>
                        </DataList.Item>

                        <DataList.Item align="center">
                            <DataList.Label>E-Mail</DataList.Label>
                            <DataList.Value style={{ flex: 1 }}>
                                <TextField.Root
                                    style={{ width: "100%" }}
                                    type="text"
                                    value="jonas.schneider@techflow.io"
                                >
                                </TextField.Root>
                            </DataList.Value>
                        </DataList.Item>

                        <DataList.Item align="center">
                            <DataList.Label>Language</DataList.Label>
                            <DataList.Value style={{ flex: 1 }}>
                                <Select.Root defaultValue="de" >
                                    <Select.Trigger variant="surface" style={{ width: "100%" }}/>
                                    <Select.Content style={{ width: "100%" }}>
                                        <Select.Item value="de">Deutsch</Select.Item>
                                        <Select.Item value="en">Englisch</Select.Item>
                                    </Select.Content>
                                </Select.Root>
                            </DataList.Value>
                        </DataList.Item>
                    </DataList.Root>
                </Flex>
            </Flex>
            <Flex style={{"justifyContent": "end"}}>
                <Button>Save</Button>
            </Flex>

        </Card>
    )
}