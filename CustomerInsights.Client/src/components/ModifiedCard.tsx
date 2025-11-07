import {Card, DataList, Flex, Text} from "@radix-ui/themes";
import LookupField from "./LookupField";

export default function ModifiedCard () {
    return (
        <Card style={{ flex: 1, minWidth: "100%" }}  mb="4">
            <Flex direction="row" style={{justifyContent: "space-between", marginLeft: "1rem", marginRight: "1rem"}} >
                <DataList.Root size="3">
                    <DataList.Item align="center">
                        <DataList.Label>Created At</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Text>01.11.2025</Text>
                        </DataList.Value>
                    </DataList.Item>

                    <DataList.Item align="center">
                        <DataList.Label>Created By</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <LookupField title="Dataverse Interface" description="" targetUrl="/accounts/123" iconUrl="" iconFallback="DI"/>
                        </DataList.Value>
                    </DataList.Item>
                </DataList.Root>

                <DataList.Root size="3">
                    <DataList.Item align="center">
                        <DataList.Label>Modified At</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Text>06.11.2025</Text>
                        </DataList.Value>
                    </DataList.Item>

                    <DataList.Item align="center">
                        <DataList.Label>Modified By</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <LookupField title="Max Mustermann" description="max.mustermann@test-gmbh.com" targetUrl="/accounts/123" iconUrl="" iconFallback="MM"/>
                        </DataList.Value>
                    </DataList.Item>
                </DataList.Root>
            </Flex>
        </Card>
    )
}