import Image from "next/image";
import monkeyImage from "../../images/monkey.png";

const Monkey = () => {
    return (
        <>
			<h1>Chaos Monkey: TBD </h1>
            <Image src={monkeyImage} width={300} alt="Chaos Monkey" />
        </>
    );
};

export default Monkey;
