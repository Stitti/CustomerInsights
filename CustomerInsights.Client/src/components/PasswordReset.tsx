import {Button, Card, Flex, Text, TextField} from "@radix-ui/themes";
import PasswordField from "./PasswordField";

export default function PasswordReset() {

    return (
        <Card variant="surface" size="3" mb="6">
                <Flex direction="column" gap="4" p="4">
                    <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                        <Text size="2">Current Password</Text>
                        <PasswordField
                            //value={password}
                            //onChange={evt => setPassword(evt.target.value)}
                            //placeholder={t("set_password_page.password_placeholder")}
                            //showLabel={t("login_form.show_password")}
                            //hideLabel={t("login_form.hide_password")}
                        />
                    </Flex>

                    {/* Lastname */}
                    <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                        <Text size="2">New Password</Text>
                        <PasswordField
                            //value={password}
                            //onChange={evt => setPassword(evt.target.value)}
                            //placeholder={t("set_password_page.password_placeholder")}
                            //showLabel={t("login_form.show_password")}
                            //hideLabel={t("login_form.hide_password")}
                        />
                    </Flex>

                    {/* Email */}
                    <Flex direction="column" gap="1" style={{ marginBottom: "15px" }}>
                        <Text size="2">Confirm new Password</Text>
                        <PasswordField
                            //value={password}
                            //onChange={evt => setPassword(evt.target.value)}
                            //placeholder={t("set_password_page.password_placeholder")}
                            //showLabel={t("login_form.show_password")}
                            //hideLabel={t("login_form.hide_password")}
                        />
                    </Flex>

                    <Flex style={{"justifyContent": "end"}}>
                        <Button>Save</Button>
                    </Flex>
                </Flex>
        </Card>
    )
}